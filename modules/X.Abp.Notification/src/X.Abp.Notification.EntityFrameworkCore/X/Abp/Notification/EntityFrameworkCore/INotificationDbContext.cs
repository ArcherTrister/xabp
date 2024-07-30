// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.EntityFrameworkCore;

using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace X.Abp.Notification.EntityFrameworkCore;

[ConnectionStringName(AbpNotificationDbProperties.ConnectionStringName)]
public interface INotificationDbContext : IEfCoreDbContext
{
    /* Add DbSet for each Aggregate Root here. Example:
     * DbSet<Question> Questions { get; }
     */
    DbSet<NotificationGroupDefinitionRecord> NotificationGroups { get; }

    DbSet<NotificationDefinitionRecord> Notifications { get; }

    DbSet<NotificationInfo> NotificationInfos { get; }

    DbSet<UserNotificationSubscription> NotificationSubscriptions { get; }

    DbSet<PublishedNotification> PublishedNotifications { get; }

    DbSet<UserNotification> UserNotifications { get; }
}
