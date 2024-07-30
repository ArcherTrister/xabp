// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.EntityFrameworkCore;

using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace X.Abp.Notification.EntityFrameworkCore;

[ConnectionStringName(AbpNotificationDbProperties.ConnectionStringName)]
public class NotificationDbContext : AbpDbContext<NotificationDbContext>, INotificationDbContext
{
    /* Add DbSet for each Aggregate Root here. Example:
     * public DbSet<Question> Questions { get; set; }
     */
    public DbSet<NotificationGroupDefinitionRecord> NotificationGroups { get; set; }

    public DbSet<NotificationDefinitionRecord> Notifications { get; set; }

    public DbSet<NotificationInfo> NotificationInfos { get; set; }

    public DbSet<UserNotificationSubscription> NotificationSubscriptions { get; set; }

    public DbSet<PublishedNotification> PublishedNotifications { get; set; }

    public DbSet<UserNotification> UserNotifications { get; set; }

    public NotificationDbContext(DbContextOptions<NotificationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ConfigureNotification();
    }
}
