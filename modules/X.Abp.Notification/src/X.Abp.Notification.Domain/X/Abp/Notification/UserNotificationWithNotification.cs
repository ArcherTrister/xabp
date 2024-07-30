// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.Notification
{
    /// <summary>
    /// A class contains a <see cref="Notification.UserNotification"/> and related <see cref="PublishedNotification"/>.
    /// </summary>
    public class UserNotificationWithNotification
    {
        /// <summary>
        /// User notification.
        /// </summary>
        public UserNotification UserNotification { get; set; }

        /// <summary>
        /// Published notification.
        /// </summary>
        public PublishedNotification Notification { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserNotificationWithNotification"/> class.
        /// </summary>
        public UserNotificationWithNotification(UserNotification userNotification, PublishedNotification notification)
        {
            UserNotification = userNotification;
            Notification = notification;
        }
    }
}
