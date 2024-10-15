using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace X.Abp.LanguageManagement;

[ConnectionStringName(LanguageManagementDbProperties.ConnectionStringName)]
public interface ILanguageManagementMongoDbContext : IAbpMongoDbContext
{
    /* Define mongo collections here. Example:
     * IMongoCollection<Question> Questions { get; }
     */
}
