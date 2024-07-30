// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace X.Abp.Identity.EntityFrameworkCore;

[DependsOn(
    typeof(AbpIdentityProDomainModule),
    typeof(AbpIdentityEntityFrameworkCoreModule))]
public class AbpIdentityProEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<IdentityProDbContext>(options =>
        {
            options.ReplaceDbContext<IIdentityDbContext, IIdentityProDbContext>();

            options.AddRepository<IdentityUser, EfCoreIdentityUserRepository>();
        });
    }
}
