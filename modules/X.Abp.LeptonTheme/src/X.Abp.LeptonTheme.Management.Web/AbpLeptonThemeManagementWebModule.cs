// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.Authorization;
using Volo.Abp.AutoMapper;
using Volo.Abp.Http.ProxyScripting.Generators.JQuery;
using Volo.Abp.Modularity;
using Volo.Abp.UI.Navigation;
using Volo.Abp.VirtualFileSystem;

using X.Abp.LeptonTheme.Management.Localization;
using X.Abp.LeptonTheme.Management.Web.Settings;

namespace X.Abp.LeptonTheme.Management.Web
{
    [DependsOn(typeof(AbpLeptonThemeManagementApplicationContractsModule))]
    [DependsOn(typeof(AbpAspNetCoreMvcUiThemeSharedModule))]
    [DependsOn(typeof(AbpAutoMapperModule))]
    [DependsOn(typeof(AbpAuthorizationModule))]
    public class AbpLeptonThemeManagementWebModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
            {
                options.AddAssemblyResource(typeof(LeptonThemeManagementResource), typeof(AbpLeptonThemeManagementWebModule).Assembly);
            });

            PreConfigure<IMvcBuilder>(mvcBuilder =>
            {
                mvcBuilder.AddApplicationPartIfNotExists(typeof(AbpLeptonThemeManagementWebModule).Assembly);
            });
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpNavigationOptions>(options =>
            {
                options.MenuContributors.Add(new AbpLeptonThemeManagementMenuContributor());
            });

            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<AbpLeptonThemeManagementWebModule>();
            });

            context.Services.AddAutoMapperObjectMapper<AbpLeptonThemeManagementWebModule>();
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddProfile<AbpLeptonThemeManagementWebAutoMapperProfile>(validate: true);
            });

            Configure<Volo.Abp.SettingManagement.Web.Pages.SettingManagement.SettingManagementPageOptions>(options =>
            {
                options.Contributors.Add(new LeptonThemeSettingManagementPageContributor());
            });

            Configure<AbpBundlingOptions>(options =>
            {
                options.ScriptBundles
                    .Configure(typeof(Volo.Abp.SettingManagement.Web.Pages.SettingManagement.IndexModel).FullName,
                        configuration =>
                        {
                            configuration.AddFiles("/Pages/LeptonThemeManagement/Components/LeptonThemeSettingGroup/Default.js");
                            configuration.AddFiles("/client-proxies/leptonThemeManagement-proxy.js");
                        });
            });

            Configure<RazorPagesOptions>(options =>
            {
                // Configure authorization.
            });

            Configure<DynamicJavaScriptProxyOptions>(options =>
            {
                options.DisableModule(LeptonThemeManagementRemoteServiceConsts.ModuleName);
            });
        }
    }
}
