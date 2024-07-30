using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace X.Abp.Chat.MongoDB;

[ConnectionStringName(ChatDbProperties.ConnectionStringName)]
public class ChatMongoDbContext : AbpMongoDbContext, IChatMongoDbContext
{
    /* Add mongo collections here. Example:
     * public IMongoCollection<Question> Questions => Collection<Question>();
     */

    protected override void CreateModel(IMongoModelBuilder modelBuilder)
    {
        base.CreateModel(modelBuilder);

        modelBuilder.ConfigureChat();
    }
}
