// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp.Application;
using Volo.Abp.Modularity;
using Volo.Abp.ObjectExtending.Modularity;

using X.Abp.ObjectExtending;
using X.Abp.Payment.Plans;

namespace X.Abp.Payment;

[DependsOn(
    typeof(AbpPaymentDomainSharedModule),
    typeof(AbpDddApplicationContractsModule))]
public class AbpPaymentApplicationContractsModule : AbpModule
{
    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        ModuleExtensionConfigurationHelper.ApplyEntityConfigurationToApi(
            PaymentModuleExtensionConsts.ModuleName,
            PaymentModuleExtensionConsts.EntityNames.Plan,
            new Type[] { typeof(PlanDto) });

        ModuleExtensionConfigurationHelper.ApplyEntityConfigurationToApi(
            PaymentModuleExtensionConsts.ModuleName,
            PaymentModuleExtensionConsts.EntityNames.GatewayPlan,
            new Type[] { typeof(GatewayPlanDto) });
    }
}
