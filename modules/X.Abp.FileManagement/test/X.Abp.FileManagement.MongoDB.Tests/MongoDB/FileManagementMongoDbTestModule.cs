using Volo.Abp.Data;
using Volo.Abp.Modularity;

namespace X.Abp.FileManagement.MongoDB;

[DependsOn(
    typeof(FileManagementApplicationTestModule),
    typeof(FileManagementMongoDbModule)
    )]
public class FileManagementMongoDbTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpDbConnectionOptions>(options =>
        {
            options.ConnectionStrings.Default = MongoDbFixture.GetRandomConnectionString();
        });
    }
}
