// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

using OpenIddict.Abstractions;

using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Identity;
using Volo.Abp.OpenIddict.Localization;

using X.Abp.OpenIddict.ClaimDestinations;

using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace X.Abp.OpenIddict.Controllers;

public abstract class AbpOpenIdDictControllerBase : AbpController
{
    protected SignInManager<IdentityUser> SignInManager => LazyServiceProvider.LazyGetRequiredService<SignInManager<IdentityUser>>();

    protected IdentityUserManager UserManager => LazyServiceProvider.LazyGetRequiredService<IdentityUserManager>();

    protected IOpenIddictApplicationManager ApplicationManager => LazyServiceProvider.LazyGetRequiredService<IOpenIddictApplicationManager>();

    protected IOpenIddictAuthorizationManager AuthorizationManager => LazyServiceProvider.LazyGetRequiredService<IOpenIddictAuthorizationManager>();

    protected IOpenIddictScopeManager ScopeManager => LazyServiceProvider.LazyGetRequiredService<IOpenIddictScopeManager>();

    protected IOpenIddictTokenManager TokenManager => LazyServiceProvider.LazyGetRequiredService<IOpenIddictTokenManager>();

    protected AbpOpenIddictClaimDestinationsManager OpenIddictClaimDestinationsManager => LazyServiceProvider.LazyGetRequiredService<AbpOpenIddictClaimDestinationsManager>();

    protected AbpOpenIdDictControllerBase()
    {
        LocalizationResource = typeof(AbpOpenIddictResource);
    }

    protected virtual Task<OpenIddictRequest> GetOpenIddictServerRequestAsync(HttpContext httpContext)
    {
        var request = HttpContext.GetOpenIddictServerRequest() ??
                      throw new InvalidOperationException(L["TheOpenIDConnectRequestCannotBeRetrieved"]);

        return Task.FromResult(request);
    }

    protected virtual async Task<IEnumerable<string>> GetResourcesAsync(ImmutableArray<string> scopes)
    {
        var resources = new List<string>();
        if (!scopes.Any())
        {
            return resources;
        }

        await foreach (var resource in ScopeManager.ListResourcesAsync(scopes))
        {
            resources.Add(resource);
        }

        return resources;
    }

    protected virtual async Task SetClaimsDestinationsAsync(ClaimsPrincipal principal)
    {
        await OpenIddictClaimDestinationsManager.SetAsync(principal);
    }

    protected virtual async Task<bool> HasFormValueAsync(string name)
    {
        if (Request.HasFormContentType)
        {
            var form = await Request.ReadFormAsync();
            if (!string.IsNullOrEmpty(form[name]))
            {
                return true;
            }
        }

        return false;
    }

    protected virtual async Task<bool> PreSignInCheckAsync(IdentityUser user)
    {
        return user.IsActive && await SignInManager.CanSignInAsync(user) && !await UserManager.IsLockedOutAsync(user);
    }
}
