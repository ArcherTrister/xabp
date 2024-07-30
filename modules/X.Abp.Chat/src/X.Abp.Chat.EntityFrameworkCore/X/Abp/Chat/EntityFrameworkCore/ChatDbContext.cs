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
public class ChatDbContext : AbpDbContext<ChatDbContext>, IChatDbContext
{
    public DbSet<Message> ChatMessages { get; set; }

    public DbSet<ChatUser> ChatUsers { get; set; }

    public DbSet<UserMessage> ChatUserMessages { get; set; }

    public DbSet<Conversation> ChatConversations { get; set; }

    public ChatDbContext(DbContextOptions<ChatDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ConfigureChat();
    }
}
