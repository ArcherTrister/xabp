// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AutoMapper;
using Volo.Abp.Http.ProxyScripting.Generators.JQuery;
using Volo.Abp.Modularity;
using Volo.Abp.UI.Navigation;
using Volo.Abp.VirtualFileSystem;

using X.Abp.TextTemplateManagement.Localization;
using X.Abp.TextTemplateManagement.Permissions;
using X.Abp.TextTemplateManagement.Web.Menus;

namespace X.Abp.TextTemplateManagement.Web;

[DependsOn(
    typeof(AbpTextTemplateManagementApplicationContractsModule),
    typeof(AbpAspNetCoreMvcUiThemeSharedModule),
    typeof(AbpAutoMapperModule))]
public class AbpTextTemplateManagementWebModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
        {
            options.AddAssemblyResource(typeof(TextTemplateManagementResource), typeof(AbpTextTemplateManagementWebModule).Assembly);
        });

        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(AbpTextTemplateManagementWebModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new AbpTextTemplateManagementMenuContributor());
        });

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<AbpTextTemplateManagementWebModule>();
        });

        context.Services.AddAutoMapperObjectMapper<AbpTextTemplateManagementWebModule>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<AbpTextTemplateManagementWebModule>(validate: true);
        });

        Configure<RazorPagesOptions>(options =>
        {
            options.Conventions.AuthorizePage("/TextTemplates/", AbpTextTemplateManagementPermissions.TextTemplates.Default);
            options.Conventions.AuthorizePage("/TextTemplates/Index", AbpTextTemplateManagementPermissions.TextTemplates.Default);
            options.Conventions.AuthorizePage("/TextTemplates/Contents/", AbpTextTemplateManagementPermissions.TextTemplates.EditContents);
            options.Conventions.AuthorizePage("/TextTemplates/Contents/Index", AbpTextTemplateManagementPermissions.TextTemplates.EditContents);
            options.Conventions.AuthorizePage("/TextTemplates/Contents/InlineContent", AbpTextTemplateManagementPermissions.TextTemplates.EditContents);
        });

        Configure<DynamicJavaScriptProxyOptions>(options => options.DisableModule(AbpTextTemplateManagementRemoteServiceConsts.ModuleName));
    }
}
