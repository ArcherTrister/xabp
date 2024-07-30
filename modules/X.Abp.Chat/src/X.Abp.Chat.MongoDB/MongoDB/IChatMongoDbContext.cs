using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace X.Abp.Chat.MongoDB;

[ConnectionStringName(ChatDbProperties.ConnectionStringName)]
public interface IChatMongoDbContext : IAbpMongoDbContext
{
    /* Define mongo collections here. Example:
     * IMongoCollection<Question> Questions { get; }
     */
}
