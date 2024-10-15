using Volo.Abp.Data;
using Volo.Abp.Modularity;

namespace X.Abp.Identity.MongoDB;

[DependsOn(
    typeof(AbpIdentityProTestBaseModule),
    typeof(AbpIdentityProMongoDbModule)
    )]
public class AbpIdentityProMongoDbTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpDbConnectionOptions>(options =>
        {
            options.ConnectionStrings.Default = MongoDbFixture.GetRandomConnectionString();
        });
    }
}
