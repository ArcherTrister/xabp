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
using Volo.Abp.FeatureManagement;
using Volo.Abp.Http.ProxyScripting.Generators.JQuery;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.ObjectExtending.Modularity;
using Volo.Abp.Threading;
using Volo.Abp.UI.Navigation;
using Volo.Abp.VirtualFileSystem;

using X.Abp.ObjectExtending;
using X.Abp.Saas.Localization;
using X.Abp.Saas.Permissions;
using X.Abp.Saas.Web.Menus;

namespace X.Abp.Saas.Web;

[DependsOn(typeof(AbpSaasApplicationContractsModule),
    typeof(AbpAspNetCoreMvcUiThemeSharedModule),
    typeof(AbpFeatureManagementWebModule),
    typeof(AbpAutoMapperModule))]
public class AbpSaasWebModule : AbpModule
{
    private static readonly OneTimeRunner OneTimeRunner = new();

    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options => options.AddAssemblyResource(typeof(SaasResource), typeof(AbpSaasWebModule).Assembly));
        PreConfigure<IMvcBuilder>(mvcBuilder => mvcBuilder.AddApplicationPartIfNotExists(typeof(AbpSaasWebModule).Assembly));
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpNavigationOptions>(options => options.MenuContributors.Add(new AbpSaasMenuContributor()));
        Configure<AbpVirtualFileSystemOptions>(options => options.FileSets.AddEmbedded<AbpSaasWebModule>("X.Abp.Saas"));
        context.Services.AddAutoMapperObjectMapper<AbpSaasWebModule>();
        Configure<AbpAutoMapperOptions>(options => options.AddProfile<AbpSaasWebAutoMapperProfile>(true));
        Configure<RazorPagesOptions>(options =>
        {
            options.Conventions.AuthorizePage("/Saas/Tenants/Index", AbpSaasPermissions.Tenants.Default);
            options.Conventions.AuthorizePage("/Saas/Tenants/CreateModal", AbpSaasPermissions.Tenants.Create);
            options.Conventions.AuthorizePage("/Saas/Tenants/EditModal", AbpSaasPermissions.Tenants.Update);
            options.Conventions.AuthorizePage("/Saas/Tenants/ConnectionStrings", AbpSaasPermissions.Tenants.ManageConnectionStrings);
            options.Conventions.AuthorizePage("/Saas/Tenants/SetPassword", AbpSaasPermissions.Tenants.SetPassword);
            options.Conventions.AuthorizePage("/Saas/Editions/Index", AbpSaasPermissions.Editions.Default);
            options.Conventions.AuthorizePage("/Saas/Editions/CreateModal", AbpSaasPermissions.Editions.Create);
            options.Conventions.AuthorizePage("/Saas/Editions/EditModal", AbpSaasPermissions.Editions.Update);
        });
        Configure<AbpPageToolbarOptions>(options =>
        {
            options.Configure<Pages.Saas.Editions.IndexModel>(toolbar => toolbar.AddButton(LocalizableString.Create<SaasResource>("NewEdition"), "plus", "CreateEdition", requiredPolicyName: AbpSaasPermissions.Editions.Create));
            options.Configure<Pages.Saas.Tenants.IndexModel>(toolbar => toolbar.AddButton(LocalizableString.Create<SaasResource>("NewTenant"), "plus", "CreateTenant", requiredPolicyName: AbpSaasPermissions.Tenants.Create));
        });
        Configure<DynamicJavaScriptProxyOptions>(options => options.DisableModule(AbpSaasRemoteServiceConsts.ModuleName));
    }

    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        OneTimeRunner.Run(() =>
        {
            ModuleExtensionConfigurationHelper.ApplyEntityConfigurationToUi(
                SaasModuleExtensionConsts.ModuleName,
                SaasModuleExtensionConsts.EntityNames.Tenant,
                new Type[]
                {
                    typeof(Pages.Saas.Tenants.CreateModalModel.TenantInfoModel)
                },
                new Type[]
                {
                    typeof(Pages.Saas.Tenants.EditModalModel.TenantInfoModel)
                });

            ModuleExtensionConfigurationHelper.ApplyEntityConfigurationToUi(
                SaasModuleExtensionConsts.ModuleName,
                SaasModuleExtensionConsts.EntityNames.Edition,
                new Type[]
                {
                    typeof(Pages.Saas.Editions.CreateModalModel.EditionInfoModel)
                },
                new Type[]
                {
                    typeof(Pages.Saas.Editions.EditModalModel.EditionInfoModel)
                });
        });
    }
}
