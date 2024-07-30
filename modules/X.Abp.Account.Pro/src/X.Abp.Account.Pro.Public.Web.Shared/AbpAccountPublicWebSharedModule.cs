// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Bundling;
using Volo.Abp.Http.ProxyScripting.Generators.JQuery;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

using X.Abp.Account.Localization;
using X.Abp.Identity;

namespace X.Abp.Account.Public.Web;

[DependsOn(
    typeof(AbpAspNetCoreMultiTenancyModule),
    typeof(AbpIdentityProDomainModule),
    typeof(AbpAccountPublicApplicationContractsModule))]
public class AbpAccountPublicWebSharedModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(optionsAction =>
        {
            optionsAction.AddAssemblyResource(typeof(AccountResource),
                typeof(AbpAccountPublicApplicationContractsModule).Assembly,
                typeof(AbpAccountPublicWebSharedModule).Assembly);
        });
        PreConfigure<IMvcBuilder>(options => options.AddApplicationPartIfNotExists(typeof(AbpAccountPublicWebSharedModule).Assembly));
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options => options.FileSets.AddEmbedded<AbpAccountPublicWebSharedModule>());
        Configure<AbpBundlingOptions>(options => options.ScriptBundles.Configure(StandardBundles.Scripts.Global, configuration =>
        {
            configuration.AddFiles("/client-proxies/account-proxy.js");
            configuration.AddContributors(new AbpAccountPublicWebSharedBundleContributor());
        }));
        Configure<DynamicJavaScriptProxyOptions>(options => options.DisableModule("account"));
    }
}
