// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.EntityFrameworkCore;

using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.Users.EntityFrameworkCore;

using X.Abp.Chat.Conversations;
using X.Abp.Chat.Messages;
using X.Abp.Chat.Users;

namespace X.Abp.Chat.EntityFrameworkCore;

public static class ChatDbContextModelBuilderExtensions
{
    public static void ConfigureChat(this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        builder.Entity<ChatUser>(b =>
        {
            // Configure table & schema name
            b.ToTable(AbpChatDbProperties.DbTablePrefix + "Users", AbpChatDbProperties.DbSchema);

            b.ConfigureByConvention();
            b.ConfigureAbpUser();

            b.ApplyObjectExtensionMappings();
        });

        builder.Entity<Message>(b =>
        {
            // Configure table & schema name
            b.ToTable(AbpChatDbProperties.DbTablePrefix + "Messages", AbpChatDbProperties.DbSchema);

            b.ConfigureByConvention();

            b.Property(x => x.Text).IsRequired().HasColumnName(nameof(Message.Text)).HasMaxLength(ChatMessageConsts.MaxTextLength);
            b.Property(x => x.IsAllRead).HasColumnName(nameof(Message.IsAllRead));
            b.Property(x => x.ReadTime).HasColumnName(nameof(Message.ReadTime));

            b.ApplyObjectExtensionMappings();
        });

        builder.Entity<UserMessage>(b =>
        {
            // Configure table & schema name
            b.ToTable(AbpChatDbProperties.DbTablePrefix + "UserMessages", AbpChatDbProperties.DbSchema);

            b.ConfigureByConvention();

            b.Property(x => x.ChatMessageId).IsRequired().HasColumnName(nameof(UserMessage.ChatMessageId));
            b.Property(x => x.UserId).IsRequired().HasColumnName(nameof(UserMessage.UserId));
            b.Property(x => x.TargetUserId).HasColumnName(nameof(UserMessage.TargetUserId));
            b.Property(x => x.Side).HasColumnName(nameof(UserMessage.Side));
            b.Property(x => x.IsRead).HasColumnName(nameof(UserMessage.IsRead));
            b.Property(x => x.ReadTime).HasColumnName(nameof(UserMessage.ReadTime));

            b.HasOne<Message>().WithMany().HasForeignKey(p => p.ChatMessageId).OnDelete(DeleteBehavior.Cascade);

            b.HasIndex(x => x.UserId);
            b.HasIndex(x => new { x.UserId, x.TargetUserId });

            b.ApplyObjectExtensionMappings();
        });

        builder.Entity<Conversation>(b =>
        {
            // Configure table & schema name
            b.ToTable(AbpChatDbProperties.DbTablePrefix + "Conversations", AbpChatDbProperties.DbSchema);

            b.ConfigureByConvention();

            b.Property(x => x.UserId).IsRequired().HasColumnName(nameof(Conversation.UserId));
            b.Property(x => x.TargetUserId).HasColumnName(nameof(Conversation.TargetUserId));
            b.Property(x => x.LastMessage).HasColumnName(nameof(Conversation.LastMessage)).HasMaxLength(ChatMessageConsts.MaxTextLength);
            b.Property(x => x.LastMessageSide).HasColumnName(nameof(Conversation.LastMessageSide));
            b.Property(x => x.LastMessageDate).HasColumnName(nameof(Conversation.LastMessageDate));
            b.Property(x => x.UnreadMessageCount).HasColumnName(nameof(Conversation.UnreadMessageCount));

            b.HasIndex(x => x.UserId);

            b.ApplyObjectExtensionMappings();
        });

        builder.TryConfigureObjectExtensions<ChatDbContext>();
    }
}
