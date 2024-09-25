using Volo.Abp.Data;
using Volo.Abp.MongoDB;
using Volo.Abp.IdentityServer;

namespace X.Abp.IdentityServer.Pro.MongoDB;

[ConnectionStringName(AbpIdentityServerDbProperties.ConnectionStringName)]
public interface IProMongoDbContext : IAbpMongoDbContext
{
    /* Define mongo collections here. Example:
     * IMongoCollection<Question> Questions { get; }
     */
}
