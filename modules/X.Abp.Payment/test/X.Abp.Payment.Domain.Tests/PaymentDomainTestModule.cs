using Volo.Abp.Modularity;

namespace X.Abp.Payment;

/* Domain tests are configured to use the EF Core provider.
 * You can switch to MongoDB, however your domain tests should be
 * database independent anyway.
 */
[DependsOn(
    typeof(AbpPaymentDomainModule),
    typeof(PaymentTestBaseModule)
    )]
public class PaymentDomainTestModule : AbpModule
{

}
