// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Linq;

namespace X.Abp.Notification
{
    /// <summary>
    /// Can be used to store a simple info as notification info.
    /// </summary>
    [Serializable]
    public class NotificationSimpleInfo
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Unique notification name.
        /// </summary>
        public string NotificationName { get; set; }

        /// <summary>
        /// Notification data as JSON string.
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// Type of the JSON serialized <see cref="Data"/>.
        /// It's AssemblyQualifiedName of the type.
        /// </summary>
        public string DataTypeName { get; set; }

        /// <summary>
        /// Gets/sets entity type name, if this is an entity level notification.
        /// It's FullName of the entity type.
        /// </summary>
        public string EntityTypeName { get; set; }

        /// <summary>
        /// AssemblyQualifiedName of the entity type.
        /// </summary>
        public string EntityTypeAssemblyQualifiedName { get; set; }

        /// <summary>
        /// Gets/sets primary key of the entity, if this is an entity level notification.
        /// </summary>
        public string EntityId { get; set; }

        /// <summary>
        /// Notification severity.
        /// </summary>
        public NotificationSeverity Severity { get; set; }

        /// <summary>
        /// Target users of the notification.
        /// If this is set, it overrides subscribed users.
        /// If this is null/empty, then notification is sent to all subscribed users.
        /// </summary>
        public string UserIds { get; set; }

        /// <summary>
        /// Excluded users.
        /// This can be set to exclude some users while publishing notifications to subscribed users.
        /// It's not normally used if <see cref="UserIds"/> is not null.
        /// </summary>
        public string ExcludedUserIds { get; set; }

        /// <summary>
        /// Target tenants of the notification.
        /// Used to send notification to subscribed users of specific tenant(s).
        /// This is valid only if UserIds is null.
        /// If it's "0", then indicates to all tenants.
        /// </summary>
        public string TenantIds { get; set; }

        /// <summary>
        /// which realtime notifiers should handle this notification
        /// </summary>
        public string TargetNotifiers { get; set; }

        public virtual string[] TargetNotifiersList => TargetNotifiers.IsNullOrWhiteSpace()
            ? new string[] { }
            : TargetNotifiers.Split(NotificationConsts.NotificationTargetSeparator).ToArray();

        public DateTime CreationTime { get; set; }

        public Guid? CreatorId { get; set; }
    }
}
