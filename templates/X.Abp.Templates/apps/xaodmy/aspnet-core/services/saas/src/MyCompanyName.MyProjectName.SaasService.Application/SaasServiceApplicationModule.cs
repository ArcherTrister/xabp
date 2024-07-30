using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

using X.Abp.Payment.Admin;
using X.Abp.Saas;

namespace MyCompanyName.MyProjectName.SaasService.Application;

[DependsOn(
    typeof(SaasServiceApplicationContractsModule),
    typeof(SaasServiceDomainModule),
    typeof(AbpSaasApplicationModule),
    typeof(AbpPaymentAdminApplicationModule))]
public class SaasServiceApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAutoMapperObjectMapper<SaasServiceApplicationModule>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<SaasServiceApplicationModule>(validate: true);
        });
    }
}
