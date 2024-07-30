using Volo.Abp.Data;
using Volo.Abp.Modularity;

namespace X.Abp.Notification.MongoDB;

[DependsOn(
    typeof(NotificationTestBaseModule),
    typeof(NotificationMongoDbModule)
    )]
public class NotificationMongoDbTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpDbConnectionOptions>(options =>
        {
            options.ConnectionStrings.Default = MongoDbFixture.GetRandomConnectionString();
        });
    }
}
