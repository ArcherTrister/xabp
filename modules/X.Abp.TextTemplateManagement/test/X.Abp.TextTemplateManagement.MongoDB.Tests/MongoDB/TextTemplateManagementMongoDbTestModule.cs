using Volo.Abp.Data;
using Volo.Abp.Modularity;

namespace X.Abp.TextTemplateManagement.MongoDB;

[DependsOn(
    typeof(TextTemplateManagementApplicationTestModule),
    typeof(AbpTextTemplateManagementMongoDbModule)
    )]
public class TextTemplateManagementMongoDbTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpDbConnectionOptions>(options =>
        {
            options.ConnectionStrings.Default = MongoDbFixture.GetRandomConnectionString();
        });
    }
}
