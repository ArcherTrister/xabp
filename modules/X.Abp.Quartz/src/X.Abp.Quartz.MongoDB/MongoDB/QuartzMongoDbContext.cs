using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace X.Abp.Quartz.MongoDB;

[ConnectionStringName(QuartzDbProperties.ConnectionStringName)]
public class QuartzMongoDbContext : AbpMongoDbContext, IQuartzMongoDbContext
{
    /* Add mongo collections here. Example:
     * public IMongoCollection<Question> Questions => Collection<Question>();
     */

    protected override void CreateModel(IMongoModelBuilder modelBuilder)
    {
        base.CreateModel(modelBuilder);

        modelBuilder.ConfigureQuartz();
    }
}
