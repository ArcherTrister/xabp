// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace X.Abp.IdentityServer;

[DependsOn(
    typeof(AbpIdentityServerProDomainModule),
    typeof(AbpAutoMapperModule),
    typeof(AbpIdentityServerProApplicationContractsModule),
    typeof(AbpDddApplicationModule))]
public class AbpIdentityServerProApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAutoMapperObjectMapper<AbpIdentityServerProApplicationModule>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<AbpIdentityServerProApplicationModule>(validate: true);
        });
    }
}
