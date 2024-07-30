// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;

using Volo.Abp.Domain.Entities.Auditing;

namespace X.Abp.Notification
{
    /// <summary>
    /// Used to store a notification request.
    /// This notification is distributed to tenants and users by <see cref="INotificationDistributer"/>.
    /// </summary>
    [Serializable]
    public class NotificationInfo : AuditedAggregateRoot<Guid>
    {
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
        /// Target users of the notification.
        /// If this is set, it overrides subscribed users.
        /// If this is null/empty, then notification is sent to all subscribed users.
        /// </summary>
        public virtual string UserIds { get; protected set; }

        /// <summary>
        /// Excluded users.
        /// This can be set to exclude some users while publishing notifications to subscribed users.
        /// It's not normally used if <see cref="UserIds"/> is not null.
        /// </summary>
        public virtual string ExcludedUserIds { get; protected set; }

        /// <summary>
        /// Target tenants of the notification.
        /// Used to send notification to subscribed users of specific tenant(s).
        /// This is valid only if UserIds is null.
        /// If it's "0", then indicates to all tenants.
        /// </summary>
        public virtual string TenantIds { get; protected set; }

        /// <summary>
        /// which realtime notifiers should handle this notification
        /// </summary>
        public virtual string TargetNotifiers { get; protected set; }

        /*
        //[NotMapped]
        //public virtual List<string> TargetNotifiersList => TargetNotifiers.IsNullOrWhiteSpace()
        //    ? new List<string>()
        //    : TargetNotifiers.Split(NotificationConsts.NotificationTargetSeparator).ToList();
        */

        protected NotificationInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationInfo"/> class.
        /// </summary>
        public NotificationInfo(Guid id)
            : base(id)
        {
            Severity = NotificationSeverity.Info;
        }

        public NotificationInfo(Guid id,
            string notificationName,
            //NotificationData data = null,
            string data = null,
            //EntityIdentifier entityIdentifier = null,
            string entityId = null,
            string entityTypeName = null,
            string entityTypeAssemblyQualifiedName = null,
            NotificationSeverity severity = NotificationSeverity.Info,
            UserIdentifier[] userIds = null,
            UserIdentifier[] excludedUserIds = null,
            Guid?[] tenantIds = null,
            string[] targetNotifiers = null)
            : base(id)
        {
            NotificationName = notificationName;
            // Data = data?.ToJsonString();
            Data = data;
            DataTypeName = data?.GetType().AssemblyQualifiedName;

            //EntityId = entityIdentifier?.Id.ToJsonString();
            //EntityTypeName = entityIdentifier?.Type.FullName;
            //EntityTypeAssemblyQualifiedName = entityIdentifier?.Type.AssemblyQualifiedName;
            EntityId = entityId;
            EntityTypeName = entityTypeName;
            EntityTypeAssemblyQualifiedName = entityTypeAssemblyQualifiedName;

            Severity = severity;
            UserIds = userIds.IsNullOrEmpty() ? null : userIds.Select(uid => uid.ToUserIdentifierString()).JoinAsString(",");
            ExcludedUserIds = excludedUserIds.IsNullOrEmpty() ? null : excludedUserIds.Select(uid => uid.ToUserIdentifierString()).JoinAsString(",");
            TenantIds = GetTenantIdsAsStr(tenantIds);
            SetTargetNotifiers(targetNotifiers);

            ConcurrencyStamp = Guid.NewGuid().ToString("N");
        }

        public virtual void SetTargetNotifiers(string[] targetNotifiers)
        {
            if (targetNotifiers == null)
            {
                return;
            }

            TargetNotifiers = string.Join(NotificationConsts.NotificationTargetSeparator.ToString(), targetNotifiers);
        }

        /// <summary>
        /// Gets the string for <see cref="TenantIds"/>.
        /// </summary>
        /// <param name="tenantIds">租户Id集合</param>
        private static string GetTenantIdsAsStr(Guid?[] tenantIds)
        {
            if (tenantIds.IsNullOrEmpty())
            {
                return null;
            }

            return tenantIds
                .Select(tenantId => tenantId == null ? "null" : tenantId.ToString())
                .JoinAsString(",");
        }
    }
}
