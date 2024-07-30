// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.Notification
{
    /// <summary>
    /// Extension methods for <see cref="UserNotification"/>.
    /// </summary>
    public static class UserNotificationExtensions
    {
        /// <summary>
        /// Converts <see cref="UserNotification"/> to <see cref="UserNotificationInfo"/>.
        /// </summary>
        public static UserNotificationInfo ToUserNotificationInfo(this UserNotification userNotificationInfo, PublishedNotificationInfo publishedNotification)
        {
            return new UserNotificationInfo
            {
                Id = userNotificationInfo.Id,
                Notification = publishedNotification,
                UserId = userNotificationInfo.UserId,
                State = userNotificationInfo.State,
                TenantId = userNotificationInfo.TenantId,
                TargetNotifiers = userNotificationInfo.TargetNotifiers
            };
        }
    }
}
