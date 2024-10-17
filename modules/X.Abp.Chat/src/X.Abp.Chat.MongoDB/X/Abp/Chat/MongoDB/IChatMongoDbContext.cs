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
public interface IChatMongoDbContext : IAbpMongoDbContext
{
    /* Define mongo collections here. Example:
     * IMongoCollection<Question> Questions { get; }
     */
    IMongoCollection<Message> ChatMessages { get; }

    IMongoCollection<ChatUser> ChatUsers { get; }

    IMongoCollection<UserMessage> ChatUserMessages { get; }

    IMongoCollection<Conversation> ChatConversations { get; }
}
