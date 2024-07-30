// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace X.Abp.Notification
{
    /// <summary>
    /// Used to manage subscriptions for notifications.
    /// </summary>
    public interface IUserNotificationSubscriptionManager
    {
        /// <summary>
        /// Subscribes to a notification for given user and notification informations.
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="notificationName">Name of the notification.</param>
        /// <param name="entityIdentifier">entity identifier</param>
        /// <param name="targetNotifiers">target notifier</param>
        Task SubscribeAsync(UserIdentifier user, string notificationName, EntityIdentifier entityIdentifier = null, string[] targetNotifiers = null);

        /// <summary>
        /// Subscribes to all available notifications for given user.
        /// It does not subscribe entity related notifications.
        /// </summary>
        /// <param name="user">User.</param>
        Task SubscribeToAllAvailableNotificationsAsync(UserIdentifier user);

        /// <summary>
        /// Unsubscribes from a notification.
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="notificationName">Name of the notification.</param>
        /// <param name="entityIdentifier">entity identifier</param>
        Task UnSubscribeAsync(UserIdentifier user, string notificationName, EntityIdentifier entityIdentifier = null);

        /// <summary>
        /// Gets all subscribtions for given notification (including all tenants).
        /// This only works for single database approach in a multitenant application!
        /// </summary>
        /// <param name="notificationName">Name of the notification.</param>
        /// <param name="entityIdentifier">entity identifier</param>
        /// <param name="targetNotifiers">target notifier</param>
        Task<List<UserNotificationSubscriptionInfo>> GetSubscriptionsAsync(string notificationName, EntityIdentifier entityIdentifier = null, string[] targetNotifiers = null);

        /// <summary>
        /// Gets all subscribtions for given notification.
        /// </summary>
        /// <param name="tenantId">Tenant id. Null for the host.</param>
        /// <param name="notificationName">Name of the notification.</param>
        /// <param name="entityIdentifier">entity identifier</param>
        /// <param name="targetNotifiers">target notifier</param>
        Task<List<UserNotificationSubscriptionInfo>> GetSubscriptionsAsync(Guid? tenantId, string notificationName, EntityIdentifier entityIdentifier = null, string[] targetNotifiers = null);

        /// <summary>
        /// Gets subscribed notifications for a user.
        /// </summary>
        /// <param name="user">User.</param>
        Task<List<UserNotificationSubscriptionInfo>> GetSubscribedNotificationsAsync(UserIdentifier user);

        /// <summary>
        /// Checks if a user subscribed for a notification.
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="notificationName">Name of the notification.</param>
        /// <param name="entityIdentifier">entity identifier</param>
        /// <param name="targetNotifiers">target notifier</param>
        Task<bool> IsSubscribedAsync(UserIdentifier user, string notificationName, EntityIdentifier entityIdentifier = null, string[] targetNotifiers = null);
    }
}
