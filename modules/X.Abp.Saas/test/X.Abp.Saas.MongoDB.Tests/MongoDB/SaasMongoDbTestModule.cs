using Volo.Abp.Data;
using Volo.Abp.Modularity;

namespace X.Abp.Saas.MongoDB;

[DependsOn(
    typeof(SaasApplicationTestModule),
    typeof(SaasMongoDbModule)
    )]
public class SaasMongoDbTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpDbConnectionOptions>(options =>
        {
            options.ConnectionStrings.Default = MongoDbFixture.GetRandomConnectionString();
        });
    }
}
