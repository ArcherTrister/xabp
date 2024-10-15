using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace X.Abp.LanguageManagement;

[ConnectionStringName(LanguageManagementDbProperties.ConnectionStringName)]
public class LanguageManagementMongoDbContext : AbpMongoDbContext, ILanguageManagementMongoDbContext
{
    /* Add mongo collections here. Example:
     * public IMongoCollection<Question> Questions => Collection<Question>();
     */

    protected override void CreateModel(IMongoModelBuilder modelBuilder)
    {
        base.CreateModel(modelBuilder);

        modelBuilder.ConfigureLanguageManagement();
    }
}
