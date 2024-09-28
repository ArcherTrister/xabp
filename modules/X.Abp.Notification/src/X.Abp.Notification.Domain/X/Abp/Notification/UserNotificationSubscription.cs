// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace X.Abp.Notification
{
    /// <summary>
    /// Used to store a notification subscription.
    /// </summary>
    public class UserNotificationSubscription : CreationAuditedEntity<Guid>, IMultiTenant
    {
        /// <summary>
        /// Tenant id of the subscribed user.
        /// </summary>
        public virtual Guid? TenantId { get; set; }

        /// <summary>
        /// User Id.
        /// </summary>
        public virtual Guid UserId { get; set; }

        /// <summary>
        /// Notification unique name.
        /// </summary>
        public virtual string NotificationName { get; set; }

        /// <summary>
        /// Gets/sets entity type name, if this is an entity level notification.
        /// It's FullName of the entity type.
        /// </summary>
        public virtual string EntityTypeName { get; set; }

        /// <summary>
        /// AssemblyQualifiedName of the entity type.
        /// </summary>
        public virtual string EntityTypeAssemblyQualifiedName { get; set; }

        /// <summary>
        /// Gets/sets primary key of the entity, if this is an entity level notification.
        /// </summary>
        public virtual string EntityId { get; set; }

        /// <summary>
        /// which realtime notifiers should handle this notification
        /// </summary>
        public virtual string TargetNotifiers { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserNotificationSubscription"/> class.
        /// </summary>
        protected UserNotificationSubscription()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserNotificationSubscription"/> class.
        /// </summary>
        public UserNotificationSubscription(
            Guid id,
            Guid? tenantId,
            Guid userId,
            string notificationName,
            string entityId = null,
            string entityTypeName = null,
            string entityTypeAssemblyQualifiedName = null,
            string[] targetNotifiers = null)
            : base(id)
        {
            TenantId = tenantId;
            NotificationName = notificationName;
            UserId = userId;
            EntityId = entityId;
            EntityTypeName = entityTypeName;
            EntityTypeAssemblyQualifiedName = entityTypeAssemblyQualifiedName;
            SetTargetNotifiers(targetNotifiers);
        }

        public virtual void SetTargetNotifiers(string[] targetNotifiers)
        {
            if (targetNotifiers == null)
            {
                return;
            }

            TargetNotifiers = string.Join(NotificationConsts.NotificationTargetSeparator.ToString(), targetNotifiers);
        }
    }
}
