// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.FeatureManagement;
using Volo.Abp.Modularity;
using Volo.Abp.ObjectExtending.Modularity;
using Volo.Abp.Threading;

using X.Abp.ObjectExtending;
using X.Abp.Payment;
using X.Abp.Saas.Dtos;

namespace X.Abp.Saas;

[DependsOn(
    typeof(AbpSaasDomainSharedModule),
    typeof(AbpFeatureManagementApplicationContractsModule),
    typeof(AbpPaymentApplicationContractsModule))]
public class AbpSaasApplicationContractsModule : AbpModule
{
    private static readonly OneTimeRunner OneTimeRunner = new();

    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        OneTimeRunner.Run(() =>
        {
            ModuleExtensionConfigurationHelper.ApplyEntityConfigurationToApi(
                SaasModuleExtensionConsts.ModuleName,
                SaasModuleExtensionConsts.EntityNames.Tenant,
                getApiTypes: new[] { typeof(SaasTenantDto) },
                createApiTypes: new[] { typeof(SaasTenantCreateDto) },
                updateApiTypes: new[] { typeof(SaasTenantUpdateDto) });

            ModuleExtensionConfigurationHelper.ApplyEntityConfigurationToApi(
                SaasModuleExtensionConsts.ModuleName,
                SaasModuleExtensionConsts.EntityNames.Edition,
                getApiTypes: new[] { typeof(EditionDto) },
                createApiTypes: new[] { typeof(EditionCreateDto) },
                updateApiTypes: new[] { typeof(EditionUpdateDto) });
        });
    }
}
