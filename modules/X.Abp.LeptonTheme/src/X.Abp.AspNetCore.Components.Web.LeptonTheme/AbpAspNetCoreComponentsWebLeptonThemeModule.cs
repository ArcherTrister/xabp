// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.AspNetCore.Components.Web.Theming;
using Volo.Abp.AspNetCore.Components.Web.Theming.Routing;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

using X.Abp.LeptonTheme.Management;

namespace X.Abp.AspNetCore.Components.Web.LeptonTheme
{
    [DependsOn(
        typeof(AbpAutoMapperModule),
        typeof(AbpLeptonThemeManagementDomainSharedModule),
        typeof(AbpAspNetCoreComponentsWebThemingModule)
        )]
    public class AbpAspNetCoreComponentsWebLeptonThemeModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpRouterOptions>(options =>
            {
                options.AdditionalAssemblies.Add(typeof(AbpAspNetCoreComponentsWebLeptonThemeModule).Assembly);
            });

            context.Services.AddAutoMapperObjectMapper<AbpAspNetCoreComponentsWebLeptonThemeModule>();

            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<AbpAspNetCoreComponentsWebLeptonThemeModule>();
            });
        }
    }
}
