// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace X.Abp.OpenIddict;

[DependsOn(
typeof(AbpOpenIddictProDomainModule),
typeof(AbpOpenIddictProApplicationContractsModule),
typeof(AbpDddApplicationModule),
typeof(AbpAutoMapperModule))]
public class AbpOpenIddictProApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAutoMapperObjectMapper<AbpOpenIddictProApplicationModule>();
        Configure<AbpAutoMapperOptions>(options => options.AddMaps<AbpOpenIddictProApplicationModule>(true));
    }
}
