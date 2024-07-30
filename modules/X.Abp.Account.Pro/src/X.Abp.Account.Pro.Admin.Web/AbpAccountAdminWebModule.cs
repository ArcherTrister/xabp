// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.Http.ProxyScripting.Generators.JQuery;
using Volo.Abp.Modularity;
using Volo.Abp.SettingManagement.Web;
using Volo.Abp.SettingManagement.Web.Pages.SettingManagement;
using Volo.Abp.VirtualFileSystem;

using X.Abp.Account.Admin.Web.Settings;
using X.Abp.Account.Localization;

namespace X.Abp.Account.Admin.Web;

[DependsOn(
    typeof(AbpAccountAdminApplicationContractsModule),
    typeof(AbpSettingManagementWebModule),
    typeof(AbpAspNetCoreMvcUiThemeSharedModule))]
public class AbpAccountAdminWebModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
        {
            options.AddAssemblyResource(
                typeof(AccountResource),
                typeof(AbpAccountAdminApplicationContractsModule).Assembly,
                typeof(AbpAccountAdminWebModule).Assembly);
        });

        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(AbpAccountAdminWebModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var config = context.Services.GetConfiguration();

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<AbpAccountAdminWebModule>();
        });

        Configure<SettingManagementPageOptions>(options =>
        {
            options.Contributors.Add(new AccountSettingManagementPageContributor());
        });

        Configure<AbpBundlingOptions>(options =>
        {
            options.ScriptBundles
                .Configure(typeof(IndexModel).FullName,
                    configuration =>
                    {
                        configuration.AddFiles("/client-proxies/accountAdmin-proxy.js");
                        configuration.AddFiles("/Pages/Account/Components/AccountSettingGroup/Default.js");
                    });
        });

        Configure<DynamicJavaScriptProxyOptions>(options =>
        {
            options.DisableModule(AbpAccountAdminRemoteServiceConsts.ModuleName);
        });
    }
}
