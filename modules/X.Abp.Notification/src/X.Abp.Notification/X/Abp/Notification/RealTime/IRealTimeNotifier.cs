// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Volo.Abp.Localization;

namespace X.Abp.Notification.RealTime
{
    /// <summary>
    /// Interface to send real time notifications to users.
    /// </summary>
    public interface IRealTimeNotifier
    {
        string Name { get; }

        ILocalizableString DisplayName { get; }

        /// <summary>
        /// If true, this real time notifier will be used for sending real time notifications when it is requested. Otherwise it will not be used.
        /// <para>
        /// If false, this realtime notifier will notify any notifications.
        /// </para>
        /// </summary>
        bool UseOnlyIfRequestedAsTarget { get; }

        /// <summary>
        /// This method tries to deliver real time notifications to specified users.
        /// If a user is not online, it should ignore him.
        /// </summary>
        Task SendNotificationsAsync(UserNotificationInfo[] userNotifications);
    }
}
