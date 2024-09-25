// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Security.Claims;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

using Volo.Abp.OpenIddict;
using Volo.Abp.OpenIddict.Localization;
using Volo.Abp.Users;

using X.Abp.Identity;

using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace X.Abp.Account.Web.Pages.Account;

public class OpenIddictAuthorizeResponse
{
    public static async Task<IActionResult> GenerateAuthorizeResponseAsync(HttpContext httpContext, IdentityUser user, params Claim[] additionalClaims)
    {
        var request = httpContext.GetOpenIddictServerRequest() ?? throw new InvalidOperationException(httpContext.RequestServices.GetRequiredService<IStringLocalizer<AbpOpenIddictResource>>()["TheOpenIDConnectRequestCannotBeRetrieved"]);
        var openIddictStringLocalizer = httpContext.RequestServices.GetRequiredService<IStringLocalizer<OpenIddictAuthorizeResponse>>();
        var userManager = httpContext.RequestServices.GetRequiredService<UserManager<IdentityUser>>();
        var signInManager = httpContext.RequestServices.GetRequiredService<SignInManager<IdentityUser>>();
        var applicationManager = httpContext.RequestServices.GetRequiredService<IOpenIddictApplicationManager>();
        var authorizationManager = httpContext.RequestServices.GetRequiredService<IOpenIddictAuthorizationManager>();
        var scopeManager = httpContext.RequestServices.GetRequiredService<IOpenIddictScopeManager>();

        var application = await applicationManager.FindByClientIdAsync(request.ClientId) ?? throw new InvalidOperationException(openIddictStringLocalizer["DetailsConcerningTheCallingClientApplicationCannotBeFound"]);

        string subject = await userManager.GetUserIdAsync(user);

        List<object> authorizations = await authorizationManager.FindAsync(subject, await applicationManager.GetIdAsync(application), OpenIddictConstants.Statuses.Valid, OpenIddictConstants.AuthorizationTypes.Permanent, request.GetScopes()).ToListAsync();

        ClaimsPrincipal principal = await signInManager.CreateUserPrincipalAsync(user);

        principal.Identities.First().AddClaims(additionalClaims);
        principal.SetScopes(request.GetScopes());

        principal.SetResources(await scopeManager.ListResourcesAsync(principal.GetScopes()).ToListAsync());

        object authorization = authorizations.LastOrDefault();
        if (authorization == null)
        {
            subject = await userManager.GetUserIdAsync(user);
            authorization = await authorizationManager.CreateAsync(principal, subject, await applicationManager.GetIdAsync(application), OpenIddictConstants.AuthorizationTypes.Permanent, principal.GetScopes());
        }

        principal.SetAuthorizationId(await authorizationManager.GetIdAsync(authorization));

        await httpContext.RequestServices.GetRequiredService<AbpOpenIddictClaimsPrincipalManager>().HandleAsync(request, principal);
        string sessionId = httpContext.RequestServices.GetRequiredService<ICurrentUser>().FindSessionId();
        if (!sessionId.IsNullOrWhiteSpace())
        {
            await httpContext.RequestServices.GetRequiredService<IdentitySessionManager>().RevokeAsync(sessionId);
        }

        return new Microsoft.AspNetCore.Mvc.SignInResult(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme, principal);
    }
}
