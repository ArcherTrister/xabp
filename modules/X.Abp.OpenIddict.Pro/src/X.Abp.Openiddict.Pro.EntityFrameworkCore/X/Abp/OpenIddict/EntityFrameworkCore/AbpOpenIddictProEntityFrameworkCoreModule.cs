// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.OpenIddict.EntityFrameworkCore;

namespace X.Abp.OpenIddict.EntityFrameworkCore;

[DependsOn(
    typeof(AbpOpenIddictProDomainModule),
    typeof(AbpOpenIddictEntityFrameworkCoreModule))]
public class AbpOpenIddictProEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<OpenIddictProDbContext>(options =>
        {
            options.ReplaceDbContext<IOpenIddictDbContext, IOpenIddictProDbContext>(MultiTenancySides.Both);
        });
    }
}
