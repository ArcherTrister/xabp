using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace X.Abp.Gdpr.MongoDB;

[ConnectionStringName(GdprDbProperties.ConnectionStringName)]
public class GdprMongoDbContext : AbpMongoDbContext, IGdprMongoDbContext
{
    /* Add mongo collections here. Example:
     * public IMongoCollection<Question> Questions => Collection<Question>();
     */

    protected override void CreateModel(IMongoModelBuilder modelBuilder)
    {
        base.CreateModel(modelBuilder);

        modelBuilder.ConfigureGdpr();
    }
}
