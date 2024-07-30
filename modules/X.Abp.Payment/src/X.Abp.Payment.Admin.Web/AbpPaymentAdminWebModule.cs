// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.PageToolbars;
using Volo.Abp.AutoMapper;
using Volo.Abp.Http.ProxyScripting.Generators.JQuery;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.ObjectExtending.Modularity;
using Volo.Abp.UI.Navigation;
using Volo.Abp.VirtualFileSystem;

using X.Abp.ObjectExtending;
using X.Abp.Payment.Admin.Permissions;
using X.Abp.Payment.Admin.Web.Menus;
using X.Abp.Payment.Localization;
using X.Abp.Payment.Web;

namespace X.Abp.Payment.Admin.Web
{
    [DependsOn(typeof(AbpPaymentWebModule), typeof(AbpPaymentAdminApplicationContractsModule))]
    public class AbpPaymentAdminWebModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
            {
                options.AddAssemblyResource(typeof(PaymentResource),
                    typeof(AbpPaymentDomainSharedModule).Assembly,
                    typeof(AbpPaymentAdminWebModule).Assembly,
                    typeof(AbpPaymentAdminApplicationContractsModule).Assembly);
            });

            PreConfigure<IMvcBuilder>(mvcBuilder => mvcBuilder.AddApplicationPartIfNotExists(typeof(AbpPaymentAdminWebModule).Assembly));
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpNavigationOptions>(options =>
            options.MenuContributors.Add(new AbpPaymentAdminMenuContributor()));
            Configure<AbpVirtualFileSystemOptions>(options => options.FileSets.AddEmbedded<AbpPaymentAdminWebModule>("X.Abp.Payment.Admin.Web"));
            context.Services.AddAutoMapperObjectMapper<AbpPaymentAdminWebModule>();
            Configure<AbpAutoMapperOptions>(options => options.AddProfile<AbpPaymentAdminWebAutoMapperProfile>(true));
            Configure<RazorPagesOptions>(options =>
            {
                options.Conventions.AuthorizeFolder("/Payment/Plans/", AbpPaymentAdminPermissions.Plans.Default);
                options.Conventions.AuthorizeFolder("/Payment/Plans/CreateModal", AbpPaymentAdminPermissions.Plans.Create);
                options.Conventions.AuthorizeFolder("/Payment/Plans/UpdateModal", AbpPaymentAdminPermissions.Plans.Update);
                options.Conventions.AuthorizeFolder("/Payment/Requests/", AbpPaymentAdminPermissions.PaymentRequests.Default);
            });
            Configure<AbpPageToolbarOptions>(options =>
            {
                options.Configure<Pages.Payment.Plans.IndexModel>(toolbar => toolbar.AddButton(LocalizableString.Create<PaymentResource>("NewPlan"), "plus", "CreatePlan", requiredPolicyName: AbpPaymentAdminPermissions.Plans.Create));
                options.Configure<Pages.Payment.Plans.GatewayPlans.IndexModel>(toolbar => toolbar.AddButton(LocalizableString.Create<PaymentResource>("NewGatewayPlan"), "plus", "CreateGatewayPlan", requiredPolicyName: AbpPaymentAdminPermissions.Plans.Create));
            });
            Configure<DynamicJavaScriptProxyOptions>(options => options.DisableModule(AbpPaymentAdminRemoteServiceConsts.ModuleName));
        }

        public override void PostConfigureServices(ServiceConfigurationContext context)
        {
            ModuleExtensionConfigurationHelper.ApplyEntityConfigurationToUi(
                PaymentModuleExtensionConsts.ModuleName,
                PaymentModuleExtensionConsts.EntityNames.Plan,
                new Type[]
                {
                    typeof(Pages.Payment.Plans.PlanCreateViewModel)
                },
                new Type[]
                {
                    typeof(Pages.Payment.Plans.PlanUpdateViewModel)
                });

            ModuleExtensionConfigurationHelper.ApplyEntityConfigurationToUi(
                PaymentModuleExtensionConsts.ModuleName,
                PaymentModuleExtensionConsts.EntityNames.GatewayPlan,
                new Type[]
                {
                    typeof(Pages.Payment.Plans.GatewayPlans.GatewayPlanCreateViewModel)
                },
                new Type[]
                {
                    typeof(Pages.Payment.Plans.GatewayPlans.GatewayPlansUpdateViewModel)
                });
        }
    }
}
