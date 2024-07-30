using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace X.Abp.VersionManagement.MongoDB;

[ConnectionStringName(VersionManagementDbProperties.ConnectionStringName)]
public interface IVersionManagementMongoDbContext : IAbpMongoDbContext
{
    /* Define mongo collections here. Example:
     * IMongoCollection<Question> Questions { get; }
     */
}
