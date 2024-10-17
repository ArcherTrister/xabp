// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp;
using Volo.Abp.MongoDB;

using X.Abp.Chat.Conversations;
using X.Abp.Chat.Messages;
using X.Abp.Chat.Users;

namespace X.Abp.Chat.MongoDB;

public static class ChatMongoDbContextExtensions
{
    public static void ConfigureChat(this IMongoModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        builder.Entity<Message>(x => x.CollectionName = AbpChatDbProperties.DbTablePrefix + "Messages");
        builder.Entity<ChatUser>(x => x.CollectionName = AbpChatDbProperties.DbTablePrefix + "ChatUsers");
        builder.Entity<UserMessage>(x => x.CollectionName = AbpChatDbProperties.DbTablePrefix + "UserMessages");
        builder.Entity<Conversation>(x => x.CollectionName = AbpChatDbProperties.DbTablePrefix + "Conversations");
    }
}
