using Volo.Abp.Data;
using Volo.Abp.Modularity;

namespace X.Abp.Quartz.MongoDB;

[DependsOn(
    typeof(QuartzApplicationTestModule),
    typeof(QuartzMongoDbModule)
    )]
public class QuartzMongoDbTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpDbConnectionOptions>(options =>
        {
            options.ConnectionStrings.Default = MongoDbFixture.GetRandomConnectionString();
        });
    }
}
