// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

using DeviceDetectorNET;

using IdentityModel;

using IdentityServer4;
using IdentityServer4.Services;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Volo.Abp.AspNetCore.WebClientInfo;
using Volo.Abp.Security.Claims;
using Volo.Abp.Users;

using X.Abp.Account.Web.Extensions;
using X.Abp.Identity;

using AuthenticationOptions = Microsoft.AspNetCore.Authentication.AuthenticationOptions;

namespace X.Abp.Account.Web.Services;

public class IdentitySessionAuthenticationService : AuthenticationService
{
    protected ILogger<IdentitySessionAuthenticationService> Logger { get; }

    public IdentitySessionAuthenticationService(
        ILogger<IdentitySessionAuthenticationService> logger,
        IAuthenticationSchemeProvider schemes,
        IAuthenticationHandlerProvider handlers,
        IClaimsTransformation transform,
        IOptions<AuthenticationOptions> options)
        : base(schemes, handlers, transform, options)
    {
        Logger = logger;
    }

    /*
    "idp claim is missing" error received
    services.Configure<SecurityStampValidatorOptions>(options =>
    {
        options.OnRefreshingPrincipal = SecurityStampValidatorCallback.UpdatePrincipal;
    });
     */
    public override async Task SignInAsync(HttpContext context, string scheme, ClaimsPrincipal principal, AuthenticationProperties properties)
    {
        var defaultScheme = await Schemes.GetDefaultSignInSchemeAsync();
        var cookieSchemeName = await context.GetCookieAuthenticationSchemeAsync(Schemes);

        if ((scheme == null && defaultScheme?.Name == cookieSchemeName) || scheme == cookieSchemeName)
        {
            AugmentPrincipal(principal);

            properties ??= new AuthenticationProperties();
            await context.RequestServices.GetRequiredService<IUserSession>().CreateSessionIdAsync(principal, properties);
        }

        ClaimsIdentity claimsIdentity = principal.Identities.FirstOrDefault();
        if (claimsIdentity == null || claimsIdentity.FindSessionId() != null)
        {
            await base.SignInAsync(context, scheme, principal, properties);
            return;
        }

        // password signin principal no clientId
        if (context.Request.Query.TryGetValue(JwtClaimTypes.ClientId, out var clientId))
        {
            var sessionId = Guid.NewGuid().ToString();
            claimsIdentity.AddClaim(new Claim(AbpClaimTypes.SessionId, sessionId));
            await base.SignInAsync(context, scheme, principal, properties);

            IOptions<AbpAccountIdentityServerOptions> accountIdentityServerOptions = context.RequestServices.GetRequiredService<IOptions<AbpAccountIdentityServerOptions>>();
            if (!accountIdentityServerOptions.Value.ClientIdToDeviceMap.TryGetValue(clientId, out var device))
            {
                Logger.LogError("No device matching clientid {ClientId} was found.", clientId);
                device = "OAuth";
            }

            IWebClientInfoProvider webClientInfoProvider = context.RequestServices.GetRequiredService<IWebClientInfoProvider>();
            await context.RequestServices.GetRequiredService<IdentitySessionManager>().CreateAsync(sessionId, device, webClientInfoProvider.DeviceInfo, principal.FindUserId().Value, principal.FindTenantId(), clientId, webClientInfoProvider.ClientIpAddress);
        }
        else
        {
            await base.SignInAsync(context, scheme, principal, properties);
        }
    }

    public override async Task SignOutAsync(HttpContext context, string scheme, AuthenticationProperties properties)
    {
        var defaultScheme = await Schemes.GetDefaultSignOutSchemeAsync();
        var cookieSchemeName = await context.GetCookieAuthenticationSchemeAsync(Schemes);

        if ((scheme == null && defaultScheme?.Name == cookieSchemeName) || scheme == cookieSchemeName)
        {
            // this sets a flag used by middleware to do post-signout work.
            context.SetSignOutCalled();
        }

        var currentUser = context.RequestServices.GetRequiredService<ICurrentUser>();
        var sessionId = currentUser.FindSessionId();
        if (!sessionId.IsNullOrWhiteSpace())
        {
            await context.RequestServices.GetRequiredService<IdentitySessionManager>().RevokeAsync(sessionId);
        }
        else
        {
            Logger.LogError("SessionId is null for user: {CurrentUserId}, Do not perform a recall operation", currentUser.Id);
        }

        await base.SignOutAsync(context, scheme, properties);
    }

    private void AugmentPrincipal(ClaimsPrincipal principal)
    {
        Logger.LogDebug("Augmenting SignInContext");

        AssertRequiredClaims(principal);
        AugmentMissingClaims(principal, TimeProvider.System.GetUtcNow().UtcDateTime);
    }

    private static void AssertRequiredClaims(ClaimsPrincipal principal)
    {
        // for now, we don't allow more than one identity in the principal/cookie
        if (principal.Identities.Count() != 1)
        {
            throw new InvalidOperationException("only a single identity supported");
        }

        if (principal.FindFirst(JwtClaimTypes.Subject) == null)
        {
            throw new InvalidOperationException("sub claim is missing");
        }
    }

    private void AugmentMissingClaims(ClaimsPrincipal principal, DateTime authTime)
    {
        var identity = principal.Identities.First();

        // ASP.NET Identity issues this claim type and uses the authentication middleware name
        // such as "Google" for the value. this code is trying to correct/convert that for
        // our scenario. IOW, we take their old AuthenticationMethod value of "Google"
        // and issue it as the idp claim. we then also issue a amr with "external"
        var amr = identity.FindFirst(ClaimTypes.AuthenticationMethod);
        if (amr != null &&
            identity.FindFirst(JwtClaimTypes.IdentityProvider) == null &&
            identity.FindFirst(JwtClaimTypes.AuthenticationMethod) == null)
        {
            Logger.LogDebug("Removing amr claim with value: {Value}", amr.Value);
            identity.RemoveClaim(amr);

            Logger.LogDebug("Adding idp claim with value: {Value}", amr.Value);
            identity.AddClaim(new Claim(JwtClaimTypes.IdentityProvider, amr.Value));

            Logger.LogDebug("Adding amr claim with value: {ExternalAuthenticationMethod}", IdentitySessionAuthenticationManagerExtensions.ExternalAuthenticationMethod);
            identity.AddClaim(new Claim(JwtClaimTypes.AuthenticationMethod, IdentitySessionAuthenticationManagerExtensions.ExternalAuthenticationMethod));
        }

        if (identity.FindFirst(JwtClaimTypes.IdentityProvider) == null)
        {
            Logger.LogDebug("Adding idp claim with value: {LocalIdentityProvider}", IdentityServerConstants.LocalIdentityProvider);
            identity.AddClaim(new Claim(JwtClaimTypes.IdentityProvider, IdentityServerConstants.LocalIdentityProvider));
        }

        if (identity.FindFirst(JwtClaimTypes.AuthenticationMethod) == null)
        {
            if (identity.FindFirst(JwtClaimTypes.IdentityProvider).Value == IdentityServerConstants.LocalIdentityProvider)
            {
                Logger.LogDebug("Adding amr claim with value: {AuthenticationMethods.Password}", OidcConstants.AuthenticationMethods.Password);
                identity.AddClaim(new Claim(JwtClaimTypes.AuthenticationMethod, OidcConstants.AuthenticationMethods.Password));
            }
            else
            {
                Logger.LogDebug("Adding amr claim with value: {ExternalAuthenticationMethod}", IdentitySessionAuthenticationManagerExtensions.ExternalAuthenticationMethod);
                identity.AddClaim(new Claim(JwtClaimTypes.AuthenticationMethod, IdentitySessionAuthenticationManagerExtensions.ExternalAuthenticationMethod));
            }
        }

        if (identity.FindFirst(JwtClaimTypes.AuthenticationTime) == null)
        {
            var time = new DateTimeOffset(authTime).ToUnixTimeSeconds().ToString();

            Logger.LogDebug("Adding auth_time claim with value: {Time}", time);
            identity.AddClaim(new Claim(JwtClaimTypes.AuthenticationTime, time, ClaimValueTypes.Integer64));
        }
    }

    private static string GetClientId(HttpContext context)
    {
        if (context.Request.Query.TryGetValue(JwtClaimTypes.ClientId, out var clientIds))
        {
            return clientIds.FirstOrDefault();
        }
        else
        {
            if (context.Request.Query.TryGetValue("ReturnUrl", out var returnUrls))
            {
                return GetParameterValue(returnUrls.FirstOrDefault());
            }

            return null;
        }
    }

    private static string GetParameterValue(string returnUrl)
    {
        if (string.IsNullOrWhiteSpace(returnUrl))
        {
            return null;
        }

        // 如果查询字符串以 '?' 开头，则移除它
        if (returnUrl.StartsWith('?'))
        {
            returnUrl = returnUrl.Substring(1);
        }

        // 解析查询字符串并返回参数值
        var parameters = returnUrl.Split('&')
                              .Select(part => part.Split('='))
                              .Where(parts => parts.Length == 2)
                              .ToDictionary(parts => WebUtility.UrlDecode(parts[0]), parts => WebUtility.UrlDecode(parts[1]));

        // 返回指定参数的值
        return parameters.TryGetValue(JwtClaimTypes.ClientId, out var clientId) ? clientId : null;
    }
}
