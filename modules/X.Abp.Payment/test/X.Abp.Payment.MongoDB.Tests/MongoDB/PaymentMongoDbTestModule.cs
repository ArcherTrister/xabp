using Volo.Abp.Data;
using Volo.Abp.Modularity;

namespace X.Abp.Payment.MongoDB;

[DependsOn(
    typeof(PaymentApplicationTestModule),
    typeof(PaymentMongoDbModule)
    )]
public class PaymentMongoDbTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpDbConnectionOptions>(options =>
        {
            options.ConnectionStrings.Default = MongoDbFixture.GetRandomConnectionString();
        });
    }
}
