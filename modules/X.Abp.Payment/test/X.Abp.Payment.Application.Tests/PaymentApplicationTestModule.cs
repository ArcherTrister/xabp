using Volo.Abp.Modularity;

namespace X.Abp.Payment;

[DependsOn(
    typeof(PaymentApplicationModule),
    typeof(PaymentDomainTestModule)
    )]
public class PaymentApplicationTestModule : AbpModule
{

}
