using System;
using Volo.Abp.Data;
using Volo.Abp.Modularity;
using Volo.Abp.Uow;

namespace X.Abp.CmsKit.Pro.MongoDB;

[DependsOn(
    typeof(CmsKitProTestBaseModule),
    typeof(CmsKitProMongoDbModule)
    )]
public class CmsKitProMongoDbTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpDbConnectionOptions>(options =>
        {
            options.ConnectionStrings.Default = MongoDbFixture.GetRandomConnectionString();
        });
    }
}
