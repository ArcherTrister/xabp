using Volo.Abp.Data;
using Volo.Abp.Modularity;

using X.Abp.Identity.MongoDB;

namespace X.Abp.IdentityServer.MongoDB;

[DependsOn(
    typeof(AbpIdentityServerProTestBaseModule),
    typeof(AbpIdentityProMongoDbModule),
    typeof(AbpIdentityServerProMongoDbModule)
    )]
public class AbpIdentityServerProMongoDbTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpDbConnectionOptions>(options =>
        {
            options.ConnectionStrings.Default = MongoDbFixture.GetRandomConnectionString();
        });
    }
}
