// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using MongoDB.Driver;

using Volo.Abp.Data;
using Volo.Abp.MongoDB;

using X.Abp.Chat.Conversations;
using X.Abp.Chat.Messages;
using X.Abp.Chat.Users;

namespace X.Abp.Chat.MongoDB;

[ConnectionStringName(AbpChatDbProperties.ConnectionStringName)]
public class ChatMongoDbContext : AbpMongoDbContext, IChatMongoDbContext
{
    /* Add mongo collections here. Example:
     * public IMongoCollection<Question> Questions => Collection<Question>();
     */
    public IMongoCollection<Message> ChatMessages => Collection<Message>();

    public IMongoCollection<ChatUser> ChatUsers => Collection<ChatUser>();

    public IMongoCollection<UserMessage> ChatUserMessages => Collection<UserMessage>();

    public IMongoCollection<Conversation> ChatConversations => Collection<Conversation>();

    protected override void CreateModel(IMongoModelBuilder modelBuilder)
    {
        base.CreateModel(modelBuilder);

        modelBuilder.ConfigureChat();
    }
}
