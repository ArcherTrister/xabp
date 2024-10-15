using Volo.Abp.Data;
using Volo.Abp.MongoDB;
using Volo.Abp.IdentityServer;

namespace X.Abp.IdentityServer.MongoDB;

[ConnectionStringName(AbpIdentityServerDbProperties.ConnectionStringName)]
public class IdentityServerProMongoDbContext : AbpMongoDbContext, IIdentityServerProMongoDbContext
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
