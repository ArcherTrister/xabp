// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.Notification.Settings
{
    /// <summary>
    /// Pre-defined setting names for notification system.
    /// </summary>
    public static class NotificationSettingNames
    {
        public const string GroupName = "Abp.Notification";

        /// <summary>
        /// A top-level switch to enable/disable receiving notifications for a user.
        /// "Abp.Notification.ReceiveNotifications".
        /// </summary>
        public const string ReceiveNotifications = GroupName + ".ReceiveNotifications";
    }
}
