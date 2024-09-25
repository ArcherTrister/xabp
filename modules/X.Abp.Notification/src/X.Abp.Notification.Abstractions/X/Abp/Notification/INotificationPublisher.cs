// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using JetBrains.Annotations;

namespace X.Abp.Notification;

public interface INotificationPublisher
{
    Task PublishAsync(
        [NotNull] string notificationName,
        NotificationData data = null,
        EntityIdentifier entityIdentifier = null,
        NotificationSeverity severity = NotificationSeverity.Info,
        UserIdentifier[] userIds = null,
        UserIdentifier[] excludedUserIds = null,
        Guid?[] tenantIds = null,
        string[] targetNotifiers = null);
}
