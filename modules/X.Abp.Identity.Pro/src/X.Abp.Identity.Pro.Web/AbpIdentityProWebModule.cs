// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.PageToolbars;
using Volo.Abp.AutoMapper;
using Volo.Abp.Http.ProxyScripting.Generators.JQuery;
using Volo.Abp.Identity;
using Volo.Abp.Identity.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.ObjectExtending.Modularity;
using Volo.Abp.PermissionManagement.Web;
using Volo.Abp.SettingManagement.Web;
using Volo.Abp.SettingManagement.Web.Pages.SettingManagement;
using Volo.Abp.Threading;
using Volo.Abp.UI.Navigation;
using Volo.Abp.VirtualFileSystem;

using X.Abp.Identity.Permissions;
using X.Abp.Identity.Web.Menus;
using X.Abp.Identity.Web.Pages.Identity.Users;
using X.Abp.Identity.Web.Pages.Identity.Users.ImportToolbar;
using X.Abp.Identity.Web.Settings;

namespace X.Abp.Identity.Web;

[DependsOn(
    typeof(AbpIdentityProApplicationContractsModule),
    typeof(AbpAutoMapperModule),
    typeof(AbpPermissionManagementWebModule),
    typeof(AbpSettingManagementWebModule),
    typeof(AbpVirtualFileSystemModule),
    typeof(AbpAspNetCoreMvcUiThemeSharedModule))]
public class AbpIdentityProWebModule : AbpModule
{
    private static readonly OneTimeRunner OneTimeRunner = new();

    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
        {
            options.AddAssemblyResource(
                typeof(IdentityResource),
                typeof(AbpIdentityProWebModule).Assembly,
                typeof(AbpIdentityDomainSharedModule).Assembly,
                typeof(AbpIdentityProDomainSharedModule).Assembly,
                typeof(AbpIdentityProApplicationContractsModule).Assembly);
        });

        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(AbpIdentityProWebModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAspNetCoreMvcOptions>(options =>
        {
            options.ConventionalControllers.FormBodyBindingIgnoredTypes.Add(typeof(ImportUsersFromFileInputWithStream));
        });

        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new AbpIdentityProWebMainMenuContributor());
        });

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<AbpIdentityProWebModule>();
        });

        context.Services.AddAutoMapperObjectMapper<AbpIdentityProWebModule>();

        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddProfile<AbpIdentityProWebAutoMapperProfile>(validate: true);
        });

        Configure<RazorPagesOptions>(options =>
        {
            options.Conventions.AuthorizePage("/Identity/Users/Index", AbpIdentityProPermissions.Users.Default);
            options.Conventions.AuthorizePage("/Identity/Users/CreateModal", AbpIdentityProPermissions.Users.Create);
            options.Conventions.AuthorizePage("/Identity/Users/EditModal", AbpIdentityProPermissions.Users.Update);
            options.Conventions.AuthorizePage("/Identity/Roles/Index", AbpIdentityProPermissions.Roles.Default);
            options.Conventions.AuthorizePage("/Identity/Roles/CreateModal", AbpIdentityProPermissions.Roles.Create);
            options.Conventions.AuthorizePage("/Identity/Roles/EditModal", AbpIdentityProPermissions.Roles.Update);
            options.Conventions.AuthorizePage("/Identity/ClaimTypes/Index", AbpIdentityProPermissions.ClaimTypes.Default);
            options.Conventions.AuthorizePage("/Identity/ClaimTypes/CreateModal", AbpIdentityProPermissions.ClaimTypes.Create);
            options.Conventions.AuthorizePage("/Identity/ClaimTypes/EditModal", AbpIdentityProPermissions.ClaimTypes.Update);
            options.Conventions.AuthorizePage("/Identity/OrganizationUnits/Index", AbpIdentityProPermissions.OrganizationUnits.Default);
            options.Conventions.AuthorizePage("/Identity/OrganizationUnits/CreateModal", AbpIdentityProPermissions.OrganizationUnits.ManageOU);
            options.Conventions.AuthorizePage("/Identity/OrganizationUnits/EditModal", AbpIdentityProPermissions.OrganizationUnits.ManageOU);
        });

        Configure<SettingManagementPageOptions>(options =>
        {
            options.Contributors.Add(new IdentitySettingManagementPageContributor());
        });

        Configure<AbpBundlingOptions>(options =>
        {
            options.ScriptBundles
                .Configure(typeof(Volo.Abp.SettingManagement.Web.Pages.SettingManagement.IndexModel).FullName,
                    configuration =>
                    {
                        configuration.AddFiles("/client-proxies/identity-proxy.js");
                        configuration.AddFiles("/Pages/Identity/Components/IdentitySettingGroup/Default.js");
                    });
        });

        Configure<AbpPageToolbarOptions>(options =>
        {
            options.Configure<Pages.Identity.Users.IndexModel>(
                toolbar =>
                {
                    toolbar.AddComponent<ImportDropdownViewComponent>(requiredPolicyName: AbpIdentityProPermissions.Users.Import);

                    toolbar.AddButton(
                        LocalizableString.Create<IdentityResource>("NewUser"),
                        icon: "plus",
                        name: "CreateUser",
                        requiredPolicyName: AbpIdentityProPermissions.Users.Create);
                });

            options.Configure<Pages.Identity.Roles.IndexModel>(
                toolbar =>
                {
                    toolbar.AddButton(
                        LocalizableString.Create<IdentityResource>("NewRole"),
                        icon: "plus",
                        name: "CreateRole",
                        requiredPolicyName: AbpIdentityProPermissions.Roles.Create);
                });

            options.Configure<Pages.Identity.ClaimTypes.IndexModel>(
                toolbar =>
                {
                    toolbar.AddButton(
                        LocalizableString.Create<IdentityResource>("NewClaimType"),
                        icon: "plus",
                        name: "CreateClaimType",
                        requiredPolicyName: AbpIdentityProPermissions.ClaimTypes.Create);
                });
        });

        Configure<DynamicJavaScriptProxyOptions>(options =>
        {
            options.DisableModule(AbpIdentityProRemoteServiceConsts.ModuleName);
        });
    }

    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        OneTimeRunner.Run(() =>
        {
            ModuleExtensionConfigurationHelper
                .ApplyEntityConfigurationToUi(
                    IdentityModuleExtensionConsts.ModuleName,
                    IdentityModuleExtensionConsts.EntityNames.User,
                    createFormTypes: new[] { typeof(CreateModalModel.UserInfoViewModel) },
                    editFormTypes: new[] { typeof(EditModalModel.UserInfoViewModel) });

            ModuleExtensionConfigurationHelper
                .ApplyEntityConfigurationToUi(
                    IdentityModuleExtensionConsts.ModuleName,
                    IdentityModuleExtensionConsts.EntityNames.Role,
                    createFormTypes: new[] { typeof(Pages.Identity.Roles.CreateModalModel.RoleInfoModel) },
                    editFormTypes: new[] { typeof(Pages.Identity.Roles.EditModalModel.RoleInfoModel) });

            ModuleExtensionConfigurationHelper
                .ApplyEntityConfigurationToUi(
                    IdentityModuleExtensionConsts.ModuleName,
                    IdentityModuleExtensionConsts.EntityNames.ClaimType,
                    createFormTypes: new[] { typeof(Pages.Identity.ClaimTypes.CreateModalModel.ClaimTypeInfoModel) },
                    editFormTypes: new[] { typeof(Pages.Identity.ClaimTypes.EditModalModel.ClaimTypeInfoModel) });

            ModuleExtensionConfigurationHelper
                .ApplyEntityConfigurationToUi(
                    IdentityModuleExtensionConsts.ModuleName,
                    IdentityModuleExtensionConsts.EntityNames.OrganizationUnit,
                    createFormTypes: new[] { typeof(Pages.Identity.OrganizationUnits.CreateModalModel.OrganizationUnitInfoModel) },
                    editFormTypes: new[] { typeof(Pages.Identity.OrganizationUnits.EditModalModel.OrganizationUnitInfoModel) });
        });
    }
}
