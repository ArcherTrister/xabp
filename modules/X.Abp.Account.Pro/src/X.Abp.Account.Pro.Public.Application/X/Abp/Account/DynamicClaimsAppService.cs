// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;
using Volo.Abp.Identity;
using Volo.Abp.Security.Claims;
using Volo.Abp.Users;

namespace X.Abp.Account
{
    [Authorize]
    public class DynamicClaimsAppService : ApplicationService, IDynamicClaimsAppService
    {
        protected IdentityDynamicClaimsPrincipalContributorCache IdentityDynamicClaimsPrincipalContributorCache { get; }

        protected IAbpClaimsPrincipalFactory AbpClaimsPrincipalFactory { get; }

        protected ICurrentPrincipalAccessor PrincipalAccessor { get; }

        public DynamicClaimsAppService(
            IdentityDynamicClaimsPrincipalContributorCache identityDynamicClaimsPrincipalContributorCache,
            IAbpClaimsPrincipalFactory abpClaimsPrincipalFactory,
            ICurrentPrincipalAccessor principalAccessor)
        {
            IdentityDynamicClaimsPrincipalContributorCache =
                identityDynamicClaimsPrincipalContributorCache;
            AbpClaimsPrincipalFactory = abpClaimsPrincipalFactory;
            PrincipalAccessor = principalAccessor;
        }

        public virtual async Task RefreshAsync()
        {
            await IdentityDynamicClaimsPrincipalContributorCache.ClearAsync(
                CurrentUser.GetId(),
                CurrentUser.TenantId);
            await AbpClaimsPrincipalFactory.CreateDynamicAsync(PrincipalAccessor.Principal);
        }
    }
}
