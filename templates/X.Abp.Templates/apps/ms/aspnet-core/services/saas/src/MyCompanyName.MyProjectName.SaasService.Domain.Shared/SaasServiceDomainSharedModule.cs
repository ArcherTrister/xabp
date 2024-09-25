using MyCompanyName.MyProjectName.SaasService.Localization;

using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

using X.Abp.Payment;
using X.Abp.Saas;

namespace MyCompanyName.MyProjectName.SaasService;

[DependsOn(
    typeof(AbpSaasDomainSharedModule),
    typeof(AbpPaymentDomainSharedModule)
)]
public class SaasServiceDomainSharedModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        SaasServiceModuleExtensionConfigurator.Configure();
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<SaasServiceDomainSharedModule>();
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Add<SaasServiceResource>("en")
                .AddBaseTypes(typeof(AbpValidationResource))
                .AddVirtualJson("/Localization/SaasService");
        });
    }
}
