using Volo.Abp.Data;
using Volo.Abp.Modularity;

namespace X.Abp.Gdpr.MongoDB;

[DependsOn(
    typeof(GdprApplicationTestModule),
    typeof(GdprMongoDbModule)
    )]
public class GdprMongoDbTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpDbConnectionOptions>(options =>
        {
            options.ConnectionStrings.Default = MongoDbFixture.GetRandomConnectionString();
        });
    }
}
