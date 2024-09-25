// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Threading.Tasks;

using IdentityServer4.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

using Volo.Abp.Identity;
using Volo.Abp.IdentityServer;
using Volo.Abp.IdentityServer.AspNetIdentity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace X.Abp.Account.Web.Services;
public class ExtraClaimsProfileService : AbpProfileService
{
    protected AbpClaimsServiceOptions Options { get; }

    public ExtraClaimsProfileService(
        IOptions<AbpClaimsServiceOptions> options,
        IdentityUserManager userManager,
        IUserClaimsPrincipalFactory<IdentityUser> claimsFactory,
        ICurrentTenant currentTenant)
        : base(userManager, claimsFactory, currentTenant)
    {
        Options = options.Value;
    }

    [UnitOfWork]
    public override async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        await base.GetProfileDataAsync(context);

        var extraClaims = context.Subject.FindAll(p => Options.RequestedClaims.Contains(p.Type));
        foreach (var extraClaim in extraClaims)
        {
            context.IssuedClaims.AddIfNotContains(extraClaim);
        }
    }
}
