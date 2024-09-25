// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace X.Abp.Notification;

/// <summary>
/// Used to store (persist) notifications.
/// </summary>
public interface INotificationStore
{
    /// <summary>
    /// Inserts a notification subscription.
    /// </summary>
    Task InsertSubscriptionAsync(
        Guid? tenantId,
        Guid userId,
        string notificationName,
        EntityIdentifier entityIdentifier = null,
        string[] targetNotifiers = null);

    /// <summary>
    /// Deletes a notification subscription.
    /// </summary>
    Task DeleteSubscriptionAsync(UserIdentifier user, string notificationName, string entityTypeName, string entityId);

    /// <summary>
    /// Inserts a notification.
    /// </summary>
    Task<Guid> InsertNotificationAsync(string notificationName,
        NotificationData data = null,
        EntityIdentifier entityIdentifier = null,
        NotificationSeverity severity = NotificationSeverity.Info,
        UserIdentifier[] userIds = null,
        UserIdentifier[] excludedUserIds = null,
        Guid?[] tenantIds = null,
        string[] targetNotifiers = null);

    /// <summary>
    /// Gets a notification by Id, or returns null if not found.
    /// </summary>
    Task<NotificationSimpleInfo> GetNotificationOrNullAsync(Guid notificationId);

    /// <summary>
    /// Inserts a user notification.
    /// </summary>
    Task<UserNotificationInfo> InsertUserNotificationAsync(Guid publishedNotificationId, UserIdentifier user, string targetNotifiers, List<UserNotificationSubscriptionInfo> userNotificationSubscriptions, PublishedNotificationInfo publishedNotification, Guid? tenantId);

    /// <summary>
    /// Gets subscriptions for a notification.
    /// </summary>
    Task<List<UserNotificationSubscriptionInfo>> GetSubscriptionsAsync(string notificationName, string entityTypeName, string entityId, string[] targetNotifiers);

    /// <summary>
    /// Gets subscriptions for a notification for specified tenant(s).
    /// </summary>
    Task<List<UserNotificationSubscriptionInfo>> GetSubscriptionsAsync(Guid?[] tenantIds, string notificationName, string entityTypeName, string entityId, string[] targetNotifiers);

    /// <summary>
    /// Gets subscriptions for a user.
    /// </summary>
    Task<List<UserNotificationSubscriptionInfo>> GetSubscriptionsAsync(UserIdentifier user);

    /// <summary>
    /// Checks if a user subscribed for a notification
    /// </summary>
    Task<bool> IsSubscribedAsync(UserIdentifier user, string notificationName, string entityTypeName, string entityId, string[] targetNotifiers);

    /// <summary>
    /// Updates a user notification state.
    /// </summary>
    Task UpdateUserNotificationStateAsync(Guid? tenantId, Guid userNotificationId, UserNotificationState state);

    /// <summary>
    /// Updates all notification states for a user.
    /// </summary>
    Task UpdateAllUserNotificationStatesAsync(UserIdentifier user, UserNotificationState state);

    /// <summary>
    /// Deletes a user notification.
    /// </summary>
    Task DeleteUserNotificationAsync(Guid? tenantId, Guid userNotificationId);

    /// <summary>
    /// Deletes all notifications of a user.
    /// </summary>
    Task DeleteAllUserNotificationsAsync(UserIdentifier user, UserNotificationState? state = null, DateTime? startDate = null, DateTime? endDate = null);

    /// <summary>
    /// Gets notifications of a user.
    /// </summary>
    /// <param name="user">User.</param>
    /// <param name="state">State</param>
    /// <param name="notificationName">Notification name</param>
    /// <param name="skipCount">Skip count.</param>
    /// <param name="maxResultCount">Maximum result count.</param>
    /// <param name="startDate">List notifications published after startDateTime</param>
    /// <param name="endDate">List notifications published before startDateTime</param>
    Task<List<UserNotificationInfo>> GetUserNotificationsWithNotificationsAsync(UserIdentifier user, UserNotificationState? state = null, string notificationName = null, int skipCount = 0, int maxResultCount = int.MaxValue, DateTime? startDate = null, DateTime? endDate = null);

    /// <summary>
    /// Gets user notification count.
    /// </summary>
    /// <param name="user">User.</param>
    /// <param name="state">The state.</param>
    /// <param name="notificationName">Notification name</param>
    /// <param name="startDate">List notifications published after startDateTime</param>
    /// <param name="endDate">List notifications published before startDateTime</param>
    Task<int> GetUserNotificationCountAsync(UserIdentifier user, UserNotificationState? state = null, string notificationName = null, DateTime? startDate = null, DateTime? endDate = null);

    /// <summary>
    /// Gets a user notification.
    /// </summary>
    /// <param name="tenantId">Tenant Id</param>
    /// <param name="userNotificationId">Skip count.</param>
    Task<UserNotificationInfo> GetUserNotificationWithNotificationOrNullAsync(Guid? tenantId, Guid userNotificationId);

    /// <summary>
    /// Inserts notification for a tenant.
    /// </summary>
    Task<PublishedNotificationInfo> InsertPublishedNotificationAsync(
        Guid? tenantId,
        string notificationName,
        string data,
        string dataTypeName,
        string entityTypeName,
        string entityTypeAssemblyQualifiedName,
        string entityId,
        NotificationSeverity severity,
        DateTime creationTime,
        Guid? creatorId,
        DateTime publishedTime);

    /// <summary>
    /// Deletes the notification.
    /// </summary>
    /// <param name="notificationId">The notification.</param>
    Task DeleteNotificationAsync(Guid notificationId);

    /*
    Task<List<GetNotificationsCreatedByUserOutput>> GetNotificationsPublishedByUserAsync(UserIdentifier user, string notificationName, DateTime? startDate, DateTime? endDate);
    */
}
