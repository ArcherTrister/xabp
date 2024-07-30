// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Json;

namespace X.Abp.Notification
{
    /// <summary>
    /// Extension methods for <see cref="UserNotificationWithNotification"/>.
    /// </summary>
    public static class UserNotificationWithNotificationExtensions
    {
        /// <summary>
        /// Converts <see cref="UserNotificationWithNotification"/> to <see cref="UserNotificationInfo"/>.
        /// </summary>
        public static UserNotificationInfo ToUserNotificationInfo(this UserNotificationWithNotification userNotificationInfoWithNotificationInfo, IJsonSerializer jsonSerializer)
        {
            return userNotificationInfoWithNotificationInfo.UserNotification.ToUserNotificationInfo(
                userNotificationInfoWithNotificationInfo.Notification.ToPublishedNotificationInfo(jsonSerializer));
        }
    }
}
