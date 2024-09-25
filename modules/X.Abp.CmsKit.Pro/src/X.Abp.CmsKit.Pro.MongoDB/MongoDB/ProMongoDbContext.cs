using Volo.Abp.Data;
using Volo.Abp.MongoDB;
using Volo.CmsKit;

namespace X.Abp.CmsKit.Pro.MongoDB;

[ConnectionStringName(AbpCmsKitDbProperties.ConnectionStringName)]
public class ProMongoDbContext : AbpMongoDbContext, IProMongoDbContext
{
    /* Add mongo collections here. Example:
     * public IMongoCollection<Question> Questions => Collection<Question>();
     */

    protected override void CreateModel(IMongoModelBuilder modelBuilder)
    {
        base.CreateModel(modelBuilder);

        modelBuilder.ConfigurePro();
    }
}
