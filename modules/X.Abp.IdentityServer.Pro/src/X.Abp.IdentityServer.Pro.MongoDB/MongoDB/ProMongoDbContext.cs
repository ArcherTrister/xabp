using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace X.Abp.IdentityServer.Pro.MongoDB;

[ConnectionStringName(ProDbProperties.ConnectionStringName)]
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
