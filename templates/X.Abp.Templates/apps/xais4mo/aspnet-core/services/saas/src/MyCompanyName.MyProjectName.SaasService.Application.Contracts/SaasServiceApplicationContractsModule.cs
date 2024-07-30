using Volo.Abp.Modularity;

using X.Abp.Payment.Admin;
using X.Abp.Saas;

namespace MyCompanyName.MyProjectName.SaasService;

[DependsOn(
    //typeof(SaasTenantApplicationContractsModule),
    typeof(AbpSaasApplicationContractsModule),
    typeof(SaasServiceDomainSharedModule),
    typeof(AbpPaymentAdminApplicationContractsModule)
)]
public class SaasServiceApplicationContractsModule : AbpModule
{
}
