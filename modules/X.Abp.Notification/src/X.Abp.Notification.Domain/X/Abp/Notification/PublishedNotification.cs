// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace X.Abp.Notification
{
    /// <summary>
    /// A notification distributed to it's related tenant.
    /// </summary>
    public class PublishedNotification : CreationAuditedEntity<Guid>, IMultiTenant
    {
        /// <summary>
        /// Tenant id of the subscribed user.
        /// </summary>
        public virtual Guid? TenantId { get; protected set; }

        /// <summary>
        /// Unique notification name.
        /// </summary>
        public virtual string NotificationName { get; protected set; }

        /// <summary>
        /// Notification data as JSON string.
        /// </summary>
        public virtual string Data { get; protected set; }

        /// <summary>
        /// Type of the JSON serialized <see cref="Data"/>.
        /// It's AssemblyQualifiedName of the type.
        /// </summary>
        public virtual string DataTypeName { get; protected set; }

        /// <summary>
        /// Gets/sets entity type name, if this is an entity level notification.
        /// It's FullName of the entity type.
        /// </summary>
        public virtual string EntityTypeName { get; protected set; }

        /// <summary>
        /// AssemblyQualifiedName of the entity type.
        /// </summary>
        public virtual string EntityTypeAssemblyQualifiedName { get; protected set; }

        /// <summary>
        /// Gets/sets primary key of the entity, if this is an entity level notification.
        /// </summary>
        public virtual string EntityId { get; protected set; }

        /// <summary>
        /// Notification severity.
        /// </summary>
        public virtual NotificationSeverity Severity { get; protected set; }

        /// <summary>
        /// Notification published time.
        /// </summary>
        public virtual DateTime PublishedTime { get; protected set; }

        protected PublishedNotification()
        {
        }

        public PublishedNotification(Guid id,
            Guid? tenantId,
            string notificationName,
            string data,
            string dataTypeName,
            string entityTypeName,
            string entityTypeAssemblyQualifiedName,
            string entityId,
            NotificationSeverity severity,
            DateTime creationTime,
            Guid? creatorId,
            DateTime publishedTime)
            : base(id)
        {
            TenantId = tenantId;
            NotificationName = notificationName;
            Data = data;
            DataTypeName = dataTypeName;
            EntityTypeName = entityTypeName;
            EntityTypeAssemblyQualifiedName = entityTypeAssemblyQualifiedName;
            EntityId = entityId;
            Severity = severity;
            CreationTime = creationTime;
            CreatorId = creatorId;
            PublishedTime = publishedTime;
        }
    }
}
