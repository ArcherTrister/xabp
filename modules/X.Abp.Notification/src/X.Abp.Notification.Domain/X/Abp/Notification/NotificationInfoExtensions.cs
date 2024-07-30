// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.Notification
{
    /// <summary>
    /// Extension methods for <see cref="NotificationInfo"/>.
    /// </summary>
    public static class NotificationInfoExtensions
    {
        /// <summary>
        /// Converts <see cref="NotificationInfo"/> to <see cref="Notification"/>.
        /// </summary>
        public static NotificationSimpleInfo ToNotificationSimpleInfo(this NotificationInfo unPublishedNotification)
        {
            return new NotificationSimpleInfo
            {
                Id = unPublishedNotification.Id,
                Data = unPublishedNotification.Data,
                DataTypeName = unPublishedNotification.DataTypeName,
                EntityId = unPublishedNotification.EntityId,
                EntityTypeAssemblyQualifiedName = unPublishedNotification.EntityTypeAssemblyQualifiedName,
                EntityTypeName = unPublishedNotification.EntityTypeName,
                ExcludedUserIds = unPublishedNotification.ExcludedUserIds,
                NotificationName = unPublishedNotification.NotificationName,
                Severity = unPublishedNotification.Severity,
                TargetNotifiers = unPublishedNotification.TargetNotifiers,
                TenantIds = unPublishedNotification.TenantIds,
                UserIds = unPublishedNotification.UserIds,
                CreationTime = unPublishedNotification.CreationTime,
                CreatorId = unPublishedNotification.CreatorId
            };
        }
    }
}
