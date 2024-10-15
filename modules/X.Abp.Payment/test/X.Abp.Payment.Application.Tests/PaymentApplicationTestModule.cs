using Volo.Abp.Modularity;

namespace X.Abp.Payment;

[DependsOn(
    typeof(AbpPaymentApplicationModule),
    typeof(PaymentDomainTestModule)
    )]
public class PaymentApplicationTestModule : AbpModule
{

}
