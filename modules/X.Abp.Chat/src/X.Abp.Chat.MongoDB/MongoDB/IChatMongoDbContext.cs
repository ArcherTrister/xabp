using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace X.Abp.Chat.MongoDB;

[ConnectionStringName(AbpChatDbProperties.ConnectionStringName)]
public interface IChatMongoDbContext : IAbpMongoDbContext
{
    /* Define mongo collections here. Example:
     * IMongoCollection<Question> Questions { get; }
     */
}
