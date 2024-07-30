// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Button;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.PageToolbars;
using Volo.Abp.AutoMapper;
using Volo.Abp.Http.ProxyScripting.Generators.JQuery;
using Volo.Abp.IdentityServer.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.ObjectExtending.Modularity;
using Volo.Abp.PermissionManagement.Web;
using Volo.Abp.Threading;
using Volo.Abp.UI.Navigation;
using Volo.Abp.VirtualFileSystem;

using X.Abp.IdentityServer.Permissions;
using X.Abp.IdentityServer.Web.Menus;
using X.Abp.IdentityServer.Web.Pages.IdentityServer.ApiResources;
using X.Abp.IdentityServer.Web.Pages.IdentityServer.Clients;
using X.Abp.IdentityServer.Web.Pages.IdentityServer.IdentityResources;

namespace X.Abp.IdentityServer.Web;

[DependsOn(
    typeof(AbpIdentityServerProApplicationContractsModule),
    typeof(AbpAspNetCoreMvcUiThemeSharedModule),
    typeof(AbpPermissionManagementWebModule),
    typeof(AbpAutoMapperModule))]
public class AbpIdentityServerProWebModule : AbpModule
{
    private static readonly OneTimeRunner OneTimeRunner = new OneTimeRunner();

    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options => options.AddAssemblyResource(typeof(AbpIdentityServerResource), typeof(AbpIdentityServerProWebModule).Assembly));
        PreConfigure<IMvcBuilder>(mvcBuilder => mvcBuilder.AddApplicationPartIfNotExists(typeof(AbpIdentityServerProWebModule).Assembly));
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpNavigationOptions>(options => options.MenuContributors.Add(new AbpIdentityServerProMenuContributor()));
        Configure<AbpVirtualFileSystemOptions>(options => options.FileSets.AddEmbedded<AbpIdentityServerProWebModule>("X.Abp.IdentityServer.Web"));
        context.Services.AddAutoMapperObjectMapper<AbpIdentityServerProWebModule>();
        Configure<AbpAutoMapperOptions>(options => options.AddProfile<AbpIdentityServerProWebAutoMapperProfile>(true));
        Configure<RazorPagesOptions>(options =>
        {
            options.Conventions.AuthorizePage("/IdentityServer/Clients/Index", AbpIdentityServerProPermissions.Client.Default);
            options.Conventions.AuthorizePage("/IdentityServer/ApiResources/Index", AbpIdentityServerProPermissions.ApiResource.Default);
            options.Conventions.AuthorizePage("/IdentityServer/IdentityResources/Index", AbpIdentityServerProPermissions.IdentityResource.Default);
            options.Conventions.AuthorizePage("/IdentityServer/ApiScopes/Index", AbpIdentityServerProPermissions.ApiScope.Default);
        });
        Configure<AbpPageToolbarOptions>(options =>
        {
            options.Configure<Pages.IdentityServer.Clients.IndexModel>(toolbar => toolbar.AddButton(LocalizableString.Create<AbpIdentityServerResource>("CreateANewClient"), "plus", id: "CreateNewButtonId", requiredPolicyName: AbpIdentityServerProPermissions.Client.Create));
            options.Configure<Pages.IdentityServer.ApiResources.IndexModel>(toolbar => toolbar.AddButton(LocalizableString.Create<AbpIdentityServerResource>("CreateANewResource"), "plus", id: "CreateNewButtonId", requiredPolicyName: AbpIdentityServerProPermissions.ApiResource.Create));
            options.Configure<Pages.IdentityServer.ApiScopes.IndexModel>(toolbar => toolbar.AddButton(LocalizableString.Create<AbpIdentityServerResource>("CreateANewScope"), "plus", id: "CreateNewButtonId", requiredPolicyName: AbpIdentityServerProPermissions.ApiScope.Create));
            options.Configure<Pages.IdentityServer.IdentityResources.IndexModel>(toolbar =>
            {
                toolbar.AddButton(LocalizableString.Create<AbpIdentityServerResource>("CreateStandardResources"), "gear", id: "CreateStandardIdentityResourcesButton", type: AbpButtonType.Secondary, requiredPolicyName: AbpIdentityServerProPermissions.IdentityResource.Create);
                toolbar.AddButton(LocalizableString.Create<AbpIdentityServerResource>("CreateANewResource"), "plus", id: "CreateNewIdentityResourceButton", requiredPolicyName: AbpIdentityServerProPermissions.IdentityResource.Create);
            });
        });
        Configure<DynamicJavaScriptProxyOptions>(options => options.DisableModule(AbpIdentityServerProRemoteServiceConsts.ModuleName));
    }

    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        OneTimeRunner.Run(() =>
        {
            ModuleExtensionConfigurationHelper.ApplyEntityConfigurationToUi(
                IdentityServerModuleExtensionConsts.ModuleName,
                IdentityServerModuleExtensionConsts.EntityNames.Client,
                new Type[] { typeof(ClientCreateModalView) },
                new Type[] { typeof(ClientUpdateModalView) });

            ModuleExtensionConfigurationHelper.ApplyEntityConfigurationToUi(
                IdentityServerModuleExtensionConsts.ModuleName,
                IdentityServerModuleExtensionConsts.EntityNames.IdentityResource,
                new Type[] { typeof(IdentityResourceCreateModalView) },
                new Type[] { typeof(IdentityResourceUpdateModalView) });

            ModuleExtensionConfigurationHelper.ApplyEntityConfigurationToUi(
                IdentityServerModuleExtensionConsts.ModuleName,
                IdentityServerModuleExtensionConsts.EntityNames.ApiResource,
                new Type[] { typeof(ApiResourceCreateModalView) },
                new Type[] { typeof(ApiResourceUpdateModalView) });
        });
    }
}
