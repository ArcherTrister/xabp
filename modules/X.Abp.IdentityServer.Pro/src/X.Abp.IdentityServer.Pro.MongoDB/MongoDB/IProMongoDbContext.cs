using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace X.Abp.IdentityServer.Pro.MongoDB;

[ConnectionStringName(ProDbProperties.ConnectionStringName)]
public interface IProMongoDbContext : IAbpMongoDbContext
{
    /* Define mongo collections here. Example:
     * IMongoCollection<Question> Questions { get; }
     */
}
