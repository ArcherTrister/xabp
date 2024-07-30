using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.Modularity;

using X.Abp.Payment.Admin;
using X.Abp.Saas;

namespace MyCompanyName.MyProjectName.SaasService;

[DependsOn(
    typeof(SaasServiceApplicationContractsModule),
    //typeof(SaasTenantHttpApiClientModule),
    typeof(AbpSaasHttpApiClientModule),
    typeof(AbpPaymentAdminHttpApiClientModule)
)]
public class SaasServiceHttpApiClientModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddHttpClientProxies(
            typeof(SaasServiceApplicationContractsModule).Assembly,
            SaasServiceRemoteServiceConsts.RemoteServiceName
        );
    }
}
