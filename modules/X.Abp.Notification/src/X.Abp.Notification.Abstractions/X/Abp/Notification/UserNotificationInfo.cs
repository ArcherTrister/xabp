// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;

using Volo.Abp.MultiTenancy;

namespace X.Abp.Notification
{
    /// <summary>
    /// Represents a notification sent to a user.
    /// </summary>
    [Serializable]
    public class UserNotificationInfo : IUserIdentifier, IMultiTenant
    {
        public Guid Id { get; set; }

        /// <summary>
        /// TenantId.
        /// </summary>
        public Guid? TenantId { get; set; }

        /// <summary>
        /// User Id.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Current state of the user notification.
        /// </summary>
        public UserNotificationState State { get; set; }

        /// <summary>
        /// The published notification.
        /// </summary>
        public PublishedNotificationInfo Notification { get; set; }

        /// <summary>
        /// which realtime notifiers should handle this notification
        /// </summary>
        public string TargetNotifiers { get; set; }

        public List<string> TargetNotifiersList => TargetNotifiers.IsNullOrWhiteSpace()
            ? new List<string>()
            : TargetNotifiers.Split(NotificationConsts.NotificationTargetSeparator).ToList();

        public UserIdentifier ToUserIdentifier()
        {
            return new UserIdentifier(TenantId, UserId);
        }
    }
}
