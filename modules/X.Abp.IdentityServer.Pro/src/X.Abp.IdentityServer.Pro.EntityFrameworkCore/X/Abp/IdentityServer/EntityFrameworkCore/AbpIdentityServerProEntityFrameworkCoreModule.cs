// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.IdentityServer.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace X.Abp.IdentityServer.EntityFrameworkCore;

[DependsOn(
    typeof(AbpIdentityServerProDomainModule),
    typeof(AbpIdentityServerEntityFrameworkCoreModule))]
public class AbpIdentityServerProEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<IdentityServerProDbContext>(options =>
        {
            options.ReplaceDbContext<IIdentityServerProDbContext, IIdentityServerDbContext>();
        });
    }
}
