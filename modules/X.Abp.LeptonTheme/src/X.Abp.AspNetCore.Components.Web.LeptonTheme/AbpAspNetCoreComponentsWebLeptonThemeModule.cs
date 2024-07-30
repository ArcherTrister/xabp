using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.Components.Web.Theming;
using Volo.Abp.AspNetCore.Components.Web.Theming.Routing;
using Volo.Abp.AutoMapper;
using Volo.Abp.LeptonTheme.Management;
using Volo.Abp.Modularity;

namespace X.Abp.AspNetCore.Components.Web.LeptonTheme
{
    [DependsOn(
        typeof(AbpAutoMapperModule),
        typeof(LeptonThemeManagementDomainSharedModule),
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