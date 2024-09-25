// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Security.Claims;

using OpenIddict.Abstractions;

using Volo.Abp.DependencyInjection;
using Volo.Abp.OpenIddict;
using Volo.Abp.Security.Claims;

namespace X.Abp.Account.Web.Pages.Account;

public class OpenIddictImpersonateClaimsPrincipalHandler :
  IAbpOpenIddictClaimsPrincipalHandler,
  ITransientDependency
{
    public virtual Task HandleAsync(AbpOpenIddictClaimsPrincipalHandlerContext context)
    {
        foreach (Claim claim in context.Principal.Claims)
        {
            if (claim.Type == AbpClaimTypes.ImpersonatorTenantId || claim.Type == AbpClaimTypes.ImpersonatorTenantName || claim.Type == AbpClaimTypes.ImpersonatorUserId || claim.Type == AbpClaimTypes.ImpersonatorUserName)
            {
                claim.SetDestinations(OpenIddictConstants.Destinations.AccessToken, OpenIddictConstants.Destinations.IdentityToken);
            }
        }

        return Task.CompletedTask;
    }
}
