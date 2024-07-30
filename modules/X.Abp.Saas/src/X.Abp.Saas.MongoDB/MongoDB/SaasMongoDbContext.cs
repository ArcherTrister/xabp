using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace X.Abp.Saas.MongoDB;

[ConnectionStringName(SaasDbProperties.ConnectionStringName)]
public class SaasMongoDbContext : AbpMongoDbContext, ISaasMongoDbContext
{
    /* Add mongo collections here. Example:
     * public IMongoCollection<Question> Questions => Collection<Question>();
     */

    protected override void CreateModel(IMongoModelBuilder modelBuilder)
    {
        base.CreateModel(modelBuilder);

        modelBuilder.ConfigureSaas();
    }
}
