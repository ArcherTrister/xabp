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

using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace X.Abp.Account.Public.Web.Pages.Account;

public class OpenIddictAuthorizeResponse
{
    public static async Task<IActionResult> GenerateAuthorizeResponseAsync(HttpContext httpContext, IdentityUser user, params Claim[] additionalClaims)
    {
        var request = OpenIddictServerAspNetCoreHelpers.GetOpenIddictServerRequest(httpContext) ?? throw new InvalidOperationException(httpContext.RequestServices.GetRequiredService<IStringLocalizer<AbpOpenIddictResource>>()["TheOpenIDConnectRequestCannotBeRetrieved"]);
        var openIddictStringLocalizer = httpContext.RequestServices.GetRequiredService<IStringLocalizer<OpenIddictAuthorizeResponse>>();
        var userManager = httpContext.RequestServices.GetRequiredService<UserManager<IdentityUser>>();
        var signInManager = httpContext.RequestServices.GetRequiredService<SignInManager<IdentityUser>>();
        var applicationManager = httpContext.RequestServices.GetRequiredService<IOpenIddictApplicationManager>();
        var authorizationManager = httpContext.RequestServices.GetRequiredService<IOpenIddictAuthorizationManager>();
        var scopeManager = httpContext.RequestServices.GetRequiredService<IOpenIddictScopeManager>();

        var application = await applicationManager.FindByClientIdAsync(request.ClientId!) ?? throw new InvalidOperationException(openIddictStringLocalizer["DetailsConcerningTheCallingClientApplicationCannotBeFound"]);

        var authorizations = await authorizationManager.FindAsync(await userManager.GetUserIdAsync(user), (await applicationManager.GetIdAsync(application))!, OpenIddictConstants.Statuses.Valid, OpenIddictConstants.AuthorizationTypes.Permanent, request.GetScopes()).ToListAsync();

        var principal = await signInManager.CreateUserPrincipalAsync(user);

        var claimsIdentity = principal.Identities.First();
        claimsIdentity.AddClaims(additionalClaims);
        principal.SetScopes(request.GetScopes());
        principal.SetResources(await scopeManager.ListResourcesAsync(principal.GetScopes()).ToListAsync());
        var authorization = authorizations.LastOrDefault();
        authorization ??= await authorizationManager.CreateAsync(principal, await userManager.GetUserIdAsync(user), (await applicationManager.GetIdAsync(application))!, OpenIddictConstants.AuthorizationTypes.Permanent, principal.GetScopes());

        principal.SetAuthorizationId(await authorizationManager.GetIdAsync(authorization));
        var claimDestinationsManager = httpContext.RequestServices.GetRequiredService<AbpOpenIddictClaimsPrincipalManager>();
        await claimDestinationsManager.HandleAsync(request, principal);
        return new Microsoft.AspNetCore.Mvc.SignInResult(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme, principal);
    }
}
