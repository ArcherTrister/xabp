// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp.Authorization;
using Volo.Abp.Modularity;
using Volo.Abp.ObjectExtending.Modularity;
using Volo.Abp.VirtualFileSystem;

using X.Abp.ObjectExtending;
using X.Abp.Payment.Admin.Plans;
using X.Abp.Payment.Plans;

namespace X.Abp.Payment.Admin
{
    [DependsOn(
        typeof(AbpPaymentApplicationContractsModule),
        typeof(AbpAuthorizationModule))]
    public class AbpPaymentAdminApplicationContractsModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpVirtualFileSystemOptions>(id => id.FileSets.AddEmbedded<AbpPaymentAdminApplicationContractsModule>());
        }

        public override void PostConfigureServices(ServiceConfigurationContext context)
        {
            ModuleExtensionConfigurationHelper.ApplyEntityConfigurationToApi(
                PaymentModuleExtensionConsts.ModuleName,
                PaymentModuleExtensionConsts.EntityNames.Plan,
                new Type[] { typeof(PlanDto) },
                new Type[] { typeof(PlanCreateInput) },
                new Type[] { typeof(PlanUpdateInput) });
            ModuleExtensionConfigurationHelper.ApplyEntityConfigurationToApi(
                PaymentModuleExtensionConsts.ModuleName,
                PaymentModuleExtensionConsts.EntityNames.GatewayPlan,
                new Type[] { typeof(GatewayPlanDto) },
                new Type[] { typeof(GatewayPlanCreateInput) },
                new Type[] { typeof(GatewayPlanUpdateInput) });
        }
    }
}
