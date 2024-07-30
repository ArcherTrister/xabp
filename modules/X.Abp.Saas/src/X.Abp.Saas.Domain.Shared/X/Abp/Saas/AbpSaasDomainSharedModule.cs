// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.FeatureManagement;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Volo.Abp.VirtualFileSystem;

using X.Abp.Payment;
using X.Abp.Saas.Localization;

namespace X.Abp.Saas;

[DependsOn(
    typeof(AbpLocalizationModule),
    typeof(AbpFeatureManagementDomainSharedModule),
    typeof(AbpPaymentDomainSharedModule),
    typeof(AbpValidationModule))]
public class AbpSaasDomainSharedModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<AbpSaasDomainSharedModule>();
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources.Add<SaasResource>("en")
            .AddVirtualJson("/X/Abp/Saas/Localization/Resources");
        });

        Configure<AbpExceptionLocalizationOptions>(options =>
        {
            options.MapCodeNamespace("X.Abp.Saas", typeof(SaasResource));
        });
    }
}
