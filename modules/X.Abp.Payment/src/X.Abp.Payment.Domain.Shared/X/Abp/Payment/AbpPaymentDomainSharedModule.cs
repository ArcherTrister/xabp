// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Domain;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

using X.Abp.Payment.Localization;

namespace X.Abp.Payment;

[DependsOn(
    typeof(AbpLocalizationModule),
    typeof(AbpDddDomainModule))]
public class AbpPaymentDomainSharedModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options => options.FileSets.AddEmbedded<AbpPaymentDomainSharedModule>());

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Add<PaymentResource>("en")
                .AddVirtualJson("/X/Abp/Payment/Localization/Resources");
        });

        Configure<AbpExceptionLocalizationOptions>(options => options.MapCodeNamespace("X.Abp.Payment", typeof(PaymentResource)));
    }
}
