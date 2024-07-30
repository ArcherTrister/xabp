using Volo.Abp.Modularity;

using X.Abp.Payment;
using X.Abp.Saas;

namespace MyCompanyName.MyProjectName.SaasService;

[DependsOn(
    typeof(SaasServiceDomainSharedModule),
    typeof(AbpSaasDomainModule),
    typeof(AbpPaymentDomainModule)
)]
public class SaasServiceDomainModule : AbpModule
{
}
