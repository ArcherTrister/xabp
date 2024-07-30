// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

namespace X.Abp.Notification
{
    /// <summary>
    /// Arguments for <see cref="NotificationDistributionJob"/>.
    /// </summary>
    [Serializable]
    public class NotificationDistributionJobArgs
    {
        /// <summary>
        /// Notification Id.
        /// </summary>
        public Guid NotificationId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationDistributionJobArgs"/> class.
        /// </summary>
        public NotificationDistributionJobArgs(Guid notificationId)
        {
            NotificationId = notificationId;
        }
    }
}
