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
using Volo.Abp.ObjectExtending;
using Volo.Abp.ObjectExtending.Modularity;
using Volo.Abp.OpenIddict.Localization;
using Volo.Abp.PermissionManagement.Web;
using Volo.Abp.Threading;
using Volo.Abp.UI.Navigation;
using Volo.Abp.VirtualFileSystem;

using X.Abp.OpenIddict.Permissions;
using X.Abp.OpenIddict.Web.Menus;
using X.Abp.OpenIddict.Web.Pages.OpenIddict.Applications;
using X.Abp.OpenIddict.Web.Pages.OpenIddict.Scopes;

namespace X.Abp.OpenIddict.Web;

[DependsOn(
    typeof(AbpOpenIddictProApplicationContractsModule),
    typeof(AbpAspNetCoreMvcUiThemeSharedModule),
    typeof(AbpPermissionManagementWebModule),
    typeof(AbpAutoMapperModule))]
public class AbpOpenIddictProWebModule : AbpModule
{
    private static readonly OneTimeRunner OneTimeRunner = new();

    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options => options.AddAssemblyResource(typeof(AbpOpenIddictResource), typeof(AbpOpenIddictProWebModule).Assembly));

        PreConfigure<IMvcBuilder>(mvcBuilder => mvcBuilder.AddApplicationPartIfNotExists(typeof(AbpOpenIddictProWebModule).Assembly));
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpNavigationOptions>(options => options.MenuContributors.Add(new AbpOpenIddictProMenuContributor()));

        Configure<AbpVirtualFileSystemOptions>(options => options.FileSets.AddEmbedded<AbpOpenIddictProWebModule>());

        context.Services.AddAutoMapperObjectMapper<AbpOpenIddictProWebModule>();

        Configure<AbpAutoMapperOptions>(options => options.AddMaps<AbpOpenIddictProWebModule>(true));

        Configure<RazorPagesOptions>(options =>
        {
            options.Conventions.AuthorizePage("/OpenIddict/Applications/Index", AbpOpenIddictProPermissions.Application.Default);
            options.Conventions.AuthorizePage("/OpenIddict/Applications/CreateModal", AbpOpenIddictProPermissions.Application.Create);
            options.Conventions.AuthorizePage("/OpenIddict/Applications/EditModal", AbpOpenIddictProPermissions.Application.Update);
            options.Conventions.AuthorizePage("/OpenIddict/Scopes/Index", AbpOpenIddictProPermissions.Scope.Default);
            options.Conventions.AuthorizePage("/OpenIddict/Scopes/CreateModal", AbpOpenIddictProPermissions.Scope.Create);
            options.Conventions.AuthorizePage("/OpenIddict/Scopes/EditModal", AbpOpenIddictProPermissions.Scope.Update);
        });

        Configure<AbpPageToolbarOptions>(options =>
        {
            options.Configure<Pages.OpenIddict.Applications.IndexModel>(toolbar => toolbar.AddButton(LocalizableString.Create<AbpOpenIddictResource>("NewApplication"), "plus", "CreateApplication", "CreateNewButtonId", requiredPolicyName: AbpOpenIddictProPermissions.Application.Create));
            options.Configure<Pages.OpenIddict.Scopes.IndexModel>(toolbar => toolbar.AddButton(LocalizableString.Create<AbpOpenIddictResource>("NewScope"), "plus", "CreateScope", "CreateNewButtonId", requiredPolicyName: AbpOpenIddictProPermissions.Scope.Create));
        });

        Configure<DynamicJavaScriptProxyOptions>(options => options.DisableModule(AbpOpenIddictProRemoteServiceConsts.ModuleName));
    }

    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        OneTimeRunner.Run(() =>
        {
            ModuleExtensionConfigurationHelper.ApplyEntityConfigurationToUi(
                OpenIddictModuleExtensionConsts.ModuleName,
                OpenIddictModuleExtensionConsts.EntityNames.Application,
                new Type[] { typeof(ApplicationCreateModalView) },
                new Type[] { typeof(ApplicationEditModalView) });

            ModuleExtensionConfigurationHelper.ApplyEntityConfigurationToUi(OpenIddictModuleExtensionConsts.ModuleName,
               OpenIddictModuleExtensionConsts.EntityNames.Scope,
               new Type[] { typeof(ScopeCreateModalView) },
               new Type[] { typeof(ScopeEditModelView) });
        });
    }
}
