using Volo.Abp.Data;
using Volo.Abp.Modularity;

namespace X.Abp.LanguageManagement.MongoDB;

[DependsOn(
    typeof(LanguageManagementApplicationTestModule),
    typeof(AbpLanguageManagementMongoDbModule)
    )]
public class LanguageManagementMongoDbTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpDbConnectionOptions>(options =>
        {
            options.ConnectionStrings.Default = MongoDbFixture.GetRandomConnectionString();
        });
    }
}
