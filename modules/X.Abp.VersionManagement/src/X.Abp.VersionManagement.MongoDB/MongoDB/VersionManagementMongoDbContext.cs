using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace X.Abp.VersionManagement.MongoDB;

[ConnectionStringName(VersionManagementDbProperties.ConnectionStringName)]
public class VersionManagementMongoDbContext : AbpMongoDbContext, IVersionManagementMongoDbContext
{
    /* Add mongo collections here. Example:
     * public IMongoCollection<Question> Questions => Collection<Question>();
     */

    protected override void CreateModel(IMongoModelBuilder modelBuilder)
    {
        base.CreateModel(modelBuilder);

        modelBuilder.ConfigureVersionManagement();
    }
}
