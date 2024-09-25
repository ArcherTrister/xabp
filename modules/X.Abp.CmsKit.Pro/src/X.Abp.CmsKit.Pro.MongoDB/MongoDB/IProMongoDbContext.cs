using Volo.Abp.Data;
using Volo.Abp.MongoDB;
using Volo.CmsKit;

namespace X.Abp.CmsKit.Pro.MongoDB;

[ConnectionStringName(AbpCmsKitDbProperties.ConnectionStringName)]
public interface IProMongoDbContext : IAbpMongoDbContext
{
    /* Define mongo collections here. Example:
     * IMongoCollection<Question> Questions { get; }
     */
}
