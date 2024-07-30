// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp.Domain.Entities;
using Volo.Abp.Json;

namespace X.Abp.Notification
{
    /// <summary>
    /// Extension methods for <see cref="PublishedNotification"/>.
    /// </summary>
    public static class PublishedNotificationExtensions
    {
        /// <summary>
        /// Converts <see cref="PublishedNotification"/> to <see cref="PublishedNotificationInfo"/>.
        /// </summary>
        public static PublishedNotificationInfo ToPublishedNotificationInfo(this PublishedNotification publishedNotification, IJsonSerializer jsonSerializer)
        {
            var entityType = publishedNotification.EntityTypeAssemblyQualifiedName.IsNullOrEmpty()
                ? null
                : Type.GetType(publishedNotification.EntityTypeAssemblyQualifiedName);

            return new PublishedNotificationInfo
            {
                Id = publishedNotification.Id,
                TenantId = publishedNotification.TenantId,
                NotificationName = publishedNotification.NotificationName,
                // Data = publishedNotification.Data.IsNullOrEmpty() ? null : JsonConvert.DeserializeObject(publishedNotification.Data, Type.GetType(publishedNotification.DataTypeName)) as NotificationData,
                Data = publishedNotification.Data.IsNullOrEmpty() ? null : jsonSerializer.Deserialize(Type.GetType(publishedNotification.DataTypeName), publishedNotification.Data) as NotificationData,
                EntityTypeName = publishedNotification.EntityTypeName,
                // EntityId = publishedNotification.EntityId.IsNullOrEmpty() ? null : JsonConvert.DeserializeObject(publishedNotification.EntityId, EntityHelper.FindPrimaryKeyType(entityType)),
                EntityId = publishedNotification.EntityId.IsNullOrEmpty() ? null : jsonSerializer.Deserialize(EntityHelper.FindPrimaryKeyType(entityType), publishedNotification.EntityId),
                Severity = publishedNotification.Severity,
                CreationTime = publishedNotification.CreationTime
            };
        }
    }
}
