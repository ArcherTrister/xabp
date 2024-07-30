// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;

using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace X.Abp.Notification
{
    /// <summary>
    /// Used to store a user notification.
    /// </summary>
    [Serializable]
    public class UserNotification : AuditedAggregateRoot<Guid>, IMultiTenant
    {
        /// <summary>
        /// Tenant Id.
        /// </summary>
        public virtual Guid? TenantId { get; set; }

        /// <summary>
        /// User Id.
        /// </summary>
        public virtual Guid UserId { get; set; }

        /// <summary>
        /// Published notification Id.
        /// </summary>
        public virtual Guid PublishedNotificationId { get; set; }

        /// <summary>
        /// Current state of the user notification.
        /// </summary>
        public virtual UserNotificationState State { get; set; }

        /// <summary>
        /// which realtime notifiers should handle this notification
        /// </summary>
        public virtual string TargetNotifiers { get; set; }

        /*
        //[NotMapped]
        //public virtual List<string> TargetNotifiersList => TargetNotifiers.IsNullOrWhiteSpace()
        //    ? new List<string>()
        //    : TargetNotifiers.Split(NotificationConsts.NotificationTargetSeparator).ToList();
        */

        protected UserNotification()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserNotification"/> class.
        /// </summary>
        public UserNotification(Guid id, DateTime creationTime)
            : base(id)
        {
            State = UserNotificationState.Unread;

            CreationTime = creationTime;
        }

        public virtual void SetTargetNotifiers(List<string> list)
        {
            TargetNotifiers = string.Join(NotificationConsts.NotificationTargetSeparator.ToString(), list);
        }
    }
}
