// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Net;
using System.Security.Claims;
using System.Security.Principal;

using IdentityModel;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

using Volo.Abp.AspNetCore.WebClientInfo;

using Volo.Abp.Security.Claims;

using Volo.Abp.Users;

using X.Abp.Account.Web;
using X.Abp.Identity;

namespace X.Abp.Account.Public.Web.Services;

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

    public override async Task SignInAsync(HttpContext context, string scheme, ClaimsPrincipal principal, AuthenticationProperties properties)
    {
        ClaimsIdentity claimsIdentity = principal.Identities.FirstOrDefault();
        if (claimsIdentity == null || claimsIdentity.FindSessionId() != null)
        {
            await base.SignInAsync(context, scheme, principal, properties);
            return;
        }

        // password signin principal no clientId
        var clientId = GetClientId(context);
        if (!clientId.IsNullOrWhiteSpace())
        {
            var sessionId = Guid.NewGuid().ToString();
            claimsIdentity.AddClaim(new Claim(AbpClaimTypes.SessionId, sessionId));
            await base.SignInAsync(context, scheme, principal, properties);

            IOptions<AbpAccountOpenIddictOptions> accountOpenIddictOptions = context.RequestServices.GetRequiredService<IOptions<AbpAccountOpenIddictOptions>>();
            if (!accountOpenIddictOptions.Value.ClientIdToDeviceMap.TryGetValue(clientId, out var device))
            {
                Logger.LogError("No device matching clientid {ClientId} was found.", clientId);
                device = "OAuth";
            }

            IWebClientInfoProvider webClientInfoProvider = context.RequestServices.GetRequiredService<IWebClientInfoProvider>();
            await context.RequestServices.GetRequiredService<IdentitySessionManager>().CreateAsync(sessionId, device, webClientInfoProvider.DeviceInfo, principal.FindUserId().Value, principal.FindTenantId(), clientId, webClientInfoProvider.ClientIpAddress);
        }
        else
        {
            Logger.LogError("ClientId is null for user: {UserId}, Do not perform a create operation", principal?.FindUserId());
            await base.SignInAsync(context, scheme, principal, properties);
        }
    }

    public override async Task SignOutAsync(HttpContext context, string scheme, AuthenticationProperties properties)
    {
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

    private static string GetClientId(HttpContext context)
    {
        if (context.Request.Query.TryGetValue("ReturnUrl", out var returnUrls))
        {
            var clientId = GetParameterValue(returnUrls.FirstOrDefault());
            if (!clientId.IsNullOrWhiteSpace())
            {
                return clientId;
            }
        }

        return context.Request.Query.TryGetValue(JwtClaimTypes.ClientId, out var clientIds) ? clientIds.FirstOrDefault() : null;

        /*
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
        */
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
