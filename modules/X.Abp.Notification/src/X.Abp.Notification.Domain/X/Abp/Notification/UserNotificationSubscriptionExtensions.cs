// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp.Domain.Entities;
using Volo.Abp.Json;

namespace X.Abp.Notification
{
    /// <summary>
    /// Extension methods for <see cref="UserNotificationSubscription"/>.
    /// </summary>
    public static class UserNotificationSubscriptionExtensions
    {
        /// <summary>
        /// Converts <see cref="UserNotification"/> to <see cref="UserNotificationInfo"/>.
        /// </summary>
        public static UserNotificationSubscriptionInfo ToUserNotificationSubscriptionInfo(this UserNotificationSubscription userNotificationSubscription, IJsonSerializer jsonSerializer)
        {
            var entityType = userNotificationSubscription.EntityTypeAssemblyQualifiedName.IsNullOrEmpty()
                ? null
                : Type.GetType(userNotificationSubscription.EntityTypeAssemblyQualifiedName);

            return new UserNotificationSubscriptionInfo
            {
                Id = userNotificationSubscription.Id,
                TenantId = userNotificationSubscription.TenantId,
                UserId = userNotificationSubscription.UserId,
                NotificationName = userNotificationSubscription.NotificationName,
                EntityType = entityType,
                EntityTypeName = userNotificationSubscription.EntityTypeName,
                EntityId = userNotificationSubscription.EntityId.IsNullOrEmpty() ? null : jsonSerializer.Deserialize(EntityHelper.FindPrimaryKeyType(entityType), userNotificationSubscription.EntityId),
                EntityTypeAssemblyQualifiedName = userNotificationSubscription.EntityTypeAssemblyQualifiedName,
                TargetNotifiers = userNotificationSubscription.TargetNotifiers,
                CreationTime = userNotificationSubscription.CreationTime
            };
        }
    }
}
