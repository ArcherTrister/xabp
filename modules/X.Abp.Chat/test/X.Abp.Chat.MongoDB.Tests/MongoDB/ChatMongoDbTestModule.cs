using Volo.Abp.Data;
using Volo.Abp.Modularity;

namespace X.Abp.Chat.MongoDB;

[DependsOn(
    typeof(ChatApplicationTestModule),
    typeof(ChatMongoDbModule)
    )]
public class ChatMongoDbTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpDbConnectionOptions>(options =>
        {
            options.ConnectionStrings.Default = MongoDbFixture.GetRandomConnectionString();
        });
    }
}
