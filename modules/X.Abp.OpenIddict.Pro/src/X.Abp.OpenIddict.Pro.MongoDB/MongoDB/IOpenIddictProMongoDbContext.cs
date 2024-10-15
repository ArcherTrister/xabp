using Volo.Abp.Data;
using Volo.Abp.MongoDB;
using Volo.Abp.OpenIddict;

namespace X.Abp.OpenIddict.MongoDB;

[ConnectionStringName(AbpOpenIddictDbProperties.ConnectionStringName)]
public interface IOpenIddictProMongoDbContext : IAbpMongoDbContext
{
    /* Define mongo collections here. Example:
     * IMongoCollection<Question> Questions { get; }
     */
}
