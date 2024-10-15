using Volo.Abp.Data;
using Volo.Abp.MongoDB;
using Volo.Abp.IdentityServer;

namespace X.Abp.IdentityServer.MongoDB;

[ConnectionStringName(AbpIdentityServerDbProperties.ConnectionStringName)]
public interface IIdentityServerProMongoDbContext : IAbpMongoDbContext
{
    /* Define mongo collections here. Example:
     * IMongoCollection<Question> Questions { get; }
     */
}
