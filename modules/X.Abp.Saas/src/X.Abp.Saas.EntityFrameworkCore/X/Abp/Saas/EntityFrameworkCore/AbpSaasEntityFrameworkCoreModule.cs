// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Modularity;

using X.Abp.Saas.Editions;
using X.Abp.Saas.Tenants;

namespace X.Abp.Saas.EntityFrameworkCore;

[DependsOn(
    typeof(AbpSaasDomainModule),
    typeof(AbpEntityFrameworkCoreModule),
    typeof(AbpFeatureManagementEntityFrameworkCoreModule))]
public class AbpSaasEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<SaasDbContext>(options =>
        {
            options.AddDefaultRepositories<ISaasDbContext>();

            options.AddRepository<Tenant, EfCoreTenantRepository>();
            options.AddRepository<Edition, EfCoreEditionRepository>();
        });
    }
}
