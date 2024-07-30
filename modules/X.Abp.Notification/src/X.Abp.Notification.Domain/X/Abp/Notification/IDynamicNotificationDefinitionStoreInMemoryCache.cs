// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace X.Abp.Notification;

public interface IDynamicNotificationDefinitionStoreInMemoryCache
{
    string CacheStamp { get; set; }

    SemaphoreSlim SyncSemaphore { get; }

    DateTime? LastCheckTime { get; set; }

    Task FillAsync(
        List<NotificationGroupDefinitionRecord> notificationGroupRecords,
        List<NotificationDefinitionRecord> notificationRecords);

    NotificationDefinition GetNotificationOrNull(string name);

    IReadOnlyList<NotificationDefinition> GetNotifications();

    IReadOnlyList<NotificationGroupDefinition> GetGroups();
}
