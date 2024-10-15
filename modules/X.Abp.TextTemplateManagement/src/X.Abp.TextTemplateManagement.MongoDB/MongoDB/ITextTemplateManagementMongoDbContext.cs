using Volo.Abp.Data;
using Volo.Abp.MongoDB;

using X.Abp.TextTemplateManagement;

namespace X.Abp.TextTemplateManagement.MongoDB;

[ConnectionStringName(TextTemplateManagementDbProperties.ConnectionStringName)]
public interface ITextTemplateManagementMongoDbContext : IAbpMongoDbContext
{
    /* Define mongo collections here. Example:
     * IMongoCollection<Question> Questions { get; }
     */
}
