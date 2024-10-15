using Volo.Abp.Data;
using Volo.Abp.Modularity;

using X.Abp.Identity.MongoDB;

namespace X.Abp.OpenIddict.MongoDB;

[DependsOn(
    typeof(AbpOpenIddictProTestBaseModule),
    typeof(AbpIdentityProMongoDbModule),
    typeof(AbpOpenIddictProMongoDbModule)
    )]
public class AbpOpenIddictProMongoDbTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpDbConnectionOptions>(options =>
        {
            options.ConnectionStrings.Default = MongoDbFixture.GetRandomConnectionString();
        });
    }
}
