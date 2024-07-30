// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.PageToolbars;
using Volo.Abp.AutoMapper;
using Volo.Abp.Http.ProxyScripting.Generators.JQuery;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.ObjectExtending.Modularity;
using Volo.Abp.Threading;
using Volo.Abp.UI.Navigation;
using Volo.Abp.VirtualFileSystem;

using X.Abp.LanguageManagement.Localization;
using X.Abp.LanguageManagement.Permissions;
using X.Abp.LanguageManagement.Web.Menus;
using X.Abp.LanguageManagement.Web.Pages.LanguageManagement;
using X.Abp.ObjectExtending;

namespace X.Abp.LanguageManagement.Web;

[DependsOn(
    typeof(AbpLanguageManagementApplicationContractsModule),
    typeof(AbpAspNetCoreMvcUiThemeSharedModule),
    typeof(AbpAutoMapperModule))]
public class AbpLanguageManagementWebModule : AbpModule
{
    private static readonly OneTimeRunner OneTimeRunner = new OneTimeRunner();

    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
        {
            options.AddAssemblyResource(typeof(LanguageManagementResource), typeof(AbpLanguageManagementWebModule).Assembly);
        });

        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(AbpLanguageManagementWebModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new AbpLanguageManagementMenuContributor());
        });

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<AbpLanguageManagementWebModule>();
        });

        context.Services.AddAutoMapperObjectMapper<AbpLanguageManagementWebModule>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<AbpLanguageManagementWebModule>(validate: true);
        });

        Configure<RazorPagesOptions>(options =>
        {
            options.Conventions.AuthorizeFolder("/LanguageManagement/", AbpLanguageManagementPermissions.Languages.Default);
            options.Conventions.AuthorizeFolder("/LanguageManagement/Edit/", AbpLanguageManagementPermissions.Languages.Edit);
            options.Conventions.AuthorizeFolder("/LanguageManagement/Create/", AbpLanguageManagementPermissions.Languages.Create);
            options.Conventions.AuthorizeFolder("/LanguageManagement/Texts/", AbpLanguageManagementPermissions.LanguageTexts.Default);
            options.Conventions.AuthorizeFolder("/LanguageManagement/Texts/Edit/", AbpLanguageManagementPermissions.LanguageTexts.Edit);
        });

        Configure<AbpPageToolbarOptions>(options => options.Configure<IndexModel>(toolbar =>
        toolbar.AddButton(LocalizableString.Create<LanguageManagementResource>("CreateNewLanguage"), "plus", id: "CreateNewLanguageButtonId", requiredPolicyName: AbpLanguageManagementPermissions.Languages.Create)));

        Configure<DynamicJavaScriptProxyOptions>(options => options.DisableModule(AbpLanguageManagementRemoteServiceConsts.ModuleName));
    }

    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        OneTimeRunner.Run(() => ModuleExtensionConfigurationHelper.ApplyEntityConfigurationToUi(
            LanguageManagementModuleExtensionConsts.ModuleName,
            LanguageManagementModuleExtensionConsts.EntityNames.Language,
            new Type[1]
            {
                typeof(CreateModel.LanguageCreateModalView)
            },
            new Type[1]
            {
                typeof(EditModel.LanguageEditModalView)
            }));
    }
}
