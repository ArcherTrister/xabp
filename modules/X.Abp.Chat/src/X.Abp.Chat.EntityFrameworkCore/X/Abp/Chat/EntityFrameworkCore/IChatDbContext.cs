// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.EntityFrameworkCore;

using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

using X.Abp.Chat.Conversations;
using X.Abp.Chat.Messages;
using X.Abp.Chat.Users;

namespace X.Abp.Chat.EntityFrameworkCore;

[ConnectionStringName(AbpChatDbProperties.ConnectionStringName)]
public interface IChatDbContext : IEfCoreDbContext
{
    DbSet<Message> ChatMessages { get; }

    DbSet<ChatUser> ChatUsers { get; }

    DbSet<UserMessage> ChatUserMessages { get; }

    DbSet<Conversation> ChatConversations { get; }
}
