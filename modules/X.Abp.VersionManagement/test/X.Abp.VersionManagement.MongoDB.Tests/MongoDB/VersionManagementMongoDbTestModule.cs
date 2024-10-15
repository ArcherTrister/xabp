using Volo.Abp.Data;
using Volo.Abp.Modularity;

namespace X.Abp.VersionManagement.MongoDB;

[DependsOn(
    typeof(VersionManagementApplicationTestModule),
    typeof(VersionManagementMongoDbModule)
    )]
public class VersionManagementMongoDbTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpDbConnectionOptions>(options =>
        {
            options.ConnectionStrings.Default = MongoDbFixture.GetRandomConnectionString();
        });
    }
}
