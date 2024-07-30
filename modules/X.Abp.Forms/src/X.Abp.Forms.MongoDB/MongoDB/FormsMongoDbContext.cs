using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace X.Abp.Forms.MongoDB;

[ConnectionStringName(FormsDbProperties.ConnectionStringName)]
public class FormsMongoDbContext : AbpMongoDbContext, IFormsMongoDbContext
{
    /* Add mongo collections here. Example:
     * public IMongoCollection<Question> Questions => Collection<Question>();
     */

    protected override void CreateModel(IMongoModelBuilder modelBuilder)
    {
        base.CreateModel(modelBuilder);

        modelBuilder.ConfigureForms();
    }
}
