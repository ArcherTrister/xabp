// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using IdentityServer4.Models;

using Microsoft.AspNetCore.Identity;

using Volo.Abp.Identity;
using Volo.Abp.IdentityServer.AspNetIdentity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

using X.Abp.Account.Public.Web.Security.Claims;

using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace X.Abp.Account.Web.AspNetIdentity;
public class CustomProfileService : AbpProfileService
{
    public CustomProfileService(
        IdentityUserManager userManager,
        IUserClaimsPrincipalFactory<IdentityUser> claimsFactory,
        ICurrentTenant currentTenant)
        : base(userManager, claimsFactory, currentTenant)
    {
    }

    [UnitOfWork]
    public override async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        await base.GetProfileDataAsync(context);

        var extraClaim = context.Subject.FindFirst(CustomClaimTypes.ProviderKey);
        if (extraClaim != null)
        {
            context.IssuedClaims.Add(extraClaim);
        }
    }
}
