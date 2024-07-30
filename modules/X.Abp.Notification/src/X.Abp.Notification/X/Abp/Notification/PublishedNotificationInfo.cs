// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp.MultiTenancy;

namespace X.Abp.Notification
{
    /// <summary>
    /// Represents a published notification for a tenant/user.
    /// </summary>
    [Serializable]
    public class PublishedNotificationInfo : IMultiTenant
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Tenant Id.
        /// </summary>
        public Guid? TenantId { get; set; }

        /// <summary>
        /// Unique notification name.
        /// </summary>
        public string NotificationName { get; set; }

        /// <summary>
        /// Notification data.
        /// </summary>
        public NotificationData Data { get; set; }

        /// <summary>
        /// Name of the entity type (including namespaces).
        /// </summary>
        public string EntityTypeName { get; set; }

        /// <summary>
        /// Entity id.
        /// </summary>
        public object EntityId { get; set; }

        /// <summary>
        /// Severity.
        /// </summary>
        public NotificationSeverity Severity { get; set; }

        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PublishedNotificationInfo"/> class.
        /// </summary>
        public PublishedNotificationInfo()
        {
            CreationTime = DateTime.Now;
        }
    }
}
