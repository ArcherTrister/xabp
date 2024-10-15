using Volo.Abp.Data;
using Volo.Abp.MongoDB;

using X.Abp.TextTemplateManagement;

namespace X.Abp.TextTemplateManagement.MongoDB;

[ConnectionStringName(TextTemplateManagementDbProperties.ConnectionStringName)]
public class TextTemplateManagementMongoDbContext : AbpMongoDbContext, ITextTemplateManagementMongoDbContext
{
    /* Add mongo collections here. Example:
     * public IMongoCollection<Question> Questions => Collection<Question>();
     */

    protected override void CreateModel(IMongoModelBuilder modelBuilder)
    {
        base.CreateModel(modelBuilder);

        modelBuilder.ConfigureTextTemplateManagement();
    }
}
