using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace X.Abp.Quartz.MongoDB;

[ConnectionStringName(QuartzDbProperties.ConnectionStringName)]
public interface IQuartzMongoDbContext : IAbpMongoDbContext
{
    /* Define mongo collections here. Example:
     * IMongoCollection<Question> Questions { get; }
     */
}
