// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Claims;

using X.Abp.Saas.Tenants;

namespace X.Abp.Saas.Editions;

public class EditionClaimsPrincipalContributor : IAbpClaimsPrincipalContributor, ITransientDependency
{
    public virtual async Task ContributeAsync(AbpClaimsPrincipalContributorContext context)
    {
        var identity = context.ClaimsPrincipal.Identities.FirstOrDefault();
        if (identity == null)
        {
            return;
        }

        var currentTenant = context.ServiceProvider.GetRequiredService<ICurrentTenant>();
        if (currentTenant.Id.HasValue)
        {
            var tenant = await context.ServiceProvider.GetRequiredService<ITenantRepository>().FindAsync(currentTenant.Id.Value);
            if (tenant != null && tenant.GetActiveEditionId().HasValue)
            {
                identity.AddOrReplace(new Claim(AbpClaimTypes.EditionId, tenant.GetActiveEditionId().ToString()));
            }
        }
    }
}
