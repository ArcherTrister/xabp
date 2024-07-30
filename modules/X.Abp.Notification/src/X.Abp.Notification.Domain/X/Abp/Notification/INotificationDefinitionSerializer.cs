// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Threading.Tasks;

using JetBrains.Annotations;

namespace X.Abp.Notification;

public interface INotificationDefinitionSerializer
{
    Task<(NotificationGroupDefinitionRecord[] NotificationGroups, NotificationDefinitionRecord[] Notifications)> SerializeAsync(IEnumerable<NotificationGroupDefinition> notificationGroups);

    Task<NotificationGroupDefinitionRecord> SerializeAsync(NotificationGroupDefinition notificationGroup);

    Task<NotificationDefinitionRecord> SerializeAsync(NotificationDefinition notification, [CanBeNull] NotificationGroupDefinition notificationGroup);
}
