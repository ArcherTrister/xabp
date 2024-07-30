// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Bundling;
using Volo.Abp.AuditLogging.Localization;
using Volo.Abp.AutoMapper;
using Volo.Abp.Http.ProxyScripting.Generators.JQuery;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.UI.Navigation;
using Volo.Abp.VirtualFileSystem;

using X.Abp.AuditLogging.Web.Menus;

namespace X.Abp.AuditLogging.Web;

[DependsOn(
    typeof(AbpAuditLoggingApplicationContractsModule),
    typeof(AbpAspNetCoreMvcUiThemeSharedModule),
    typeof(AbpAutoMapperModule))]
public class AbpAuditLoggingWebModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
        {
            options.AddAssemblyResource(typeof(AuditLoggingResource), typeof(AbpAuditLoggingWebModule).Assembly);
        });

        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(AbpAuditLoggingWebModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAutoMapperObjectMapper<AbpAuditLoggingWebModule>();
        Configure<AbpAutoMapperOptions>(options => options.AddProfile<AbpAuditLoggingWebAutoMapperProfile>(true));
        Configure<AbpVirtualFileSystemOptions>(options => options.FileSets.AddEmbedded<AbpAuditLoggingWebModule>());
        Configure<AbpLocalizationOptions>(options => options.Resources.Get<AuditLoggingResource>().AddVirtualJson("/X/Abp/Identity/Localization/ApplicationContracts"));
        Configure<RazorPagesOptions>(options => options.Conventions.AddPageRoute("/AuditLogs/Detail", "AuditLogs/Detail/{id}"));
        Configure<RazorPagesOptions>(options =>
        {
            options.Conventions.AuthorizePage("/AuditLogs/", "AuditLogging.AuditLogs");
            options.Conventions.AuthorizePage("/AuditLogs/Index", "AuditLogging.AuditLogs");
            options.Conventions.AuthorizePage("/AuditLogs/Detail", "AuditLogging.AuditLogs");
            options.Conventions.AuthorizePage("/AuditLogs/EntityChangeDetail", "AuditLogging.AuditLogs");
        });
        Configure((AbpNavigationOptions options) => options.MenuContributors.Add(new AbpAuditLoggingMainMenuContributor()));
        Configure<AbpBundlingOptions>(options => options.ScriptBundles.Configure(StandardBundles.Scripts.Global,
            configuration =>
            {
                // configuration.AddFiles("/client-proxies/auditLogging-proxy.js");
                configuration.AddFiles("/Pages/AuditLogs/audit-log-global.js");
            }));
        Configure<DynamicJavaScriptProxyOptions>(options => options.DisableModule(AbpAuditLoggingRemoteServiceConsts.ModuleName));
    }
}
