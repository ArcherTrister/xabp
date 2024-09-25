using Volo.Abp.Modularity;

using X.Abp.Payment.Admin;
using X.Abp.Saas;

namespace MyCompanyName.MyProjectName.SaasService;

[DependsOn(
    typeof(SaasServiceApplicationContractsModule),
    typeof(AbpSaasHttpApiModule),
    //typeof(SaasTenantHttpApiModule),
    typeof(AbpPaymentAdminHttpApiModule)
)]
public class SaasServiceHttpApiModule : AbpModule
{
}
