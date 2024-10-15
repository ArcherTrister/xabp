using Volo.Abp.Data;
using Volo.Abp.Modularity;

namespace X.Abp.Forms.MongoDB;

[DependsOn(
    typeof(FormsApplicationTestModule),
    typeof(FormsMongoDbModule)
    )]
public class FormsMongoDbTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpDbConnectionOptions>(options =>
        {
            options.ConnectionStrings.Default = MongoDbFixture.GetRandomConnectionString();
        });
    }
}
