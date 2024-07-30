// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Volo.Abp.Domain.Services;
using Volo.Abp.Settings;
using Volo.Abp.Uow;

using X.Abp.Notification.RealTime;
using X.Abp.Notification.Settings;

namespace X.Abp.Notification
{
    /// <summary>
    /// Used to distribute notifications to users.
    /// </summary>
    public class DefaultNotificationDistributer : DomainService, INotificationDistributer
    {
        protected INotificationStore NotificationStore { get; }

        protected IRealTimeNotifierManager RealTimeNotifierManager { get; }

        protected INotificationDefinitionManager NotificationDefinitionManager { get; }

        protected ISettingStore SettingStore { get; }

        protected IUnitOfWorkManager UnitOfWorkManager { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultNotificationDistributer"/> class.
        /// </summary>
        public DefaultNotificationDistributer(
            INotificationStore notificationStore,
            IRealTimeNotifierManager realTimeNotifierManager,
            INotificationDefinitionManager notificationDefinitionManager,
            ISettingStore settingStore,
            IUnitOfWorkManager unitOfWorkManager)
        {
            NotificationStore = notificationStore;
            RealTimeNotifierManager = realTimeNotifierManager;
            NotificationDefinitionManager = notificationDefinitionManager;
            SettingStore = settingStore;
            UnitOfWorkManager = unitOfWorkManager;
        }

        public virtual async Task DistributeAsync(Guid notificationId)
        {
            using (var uow = UnitOfWorkManager.Begin(true))
            {
                var notificationInfo = await NotificationStore.GetNotificationOrNullAsync(notificationId);
                if (notificationInfo == null)
                {
                    Logger.LogWarning(
                        "NotificationDistributionJob can not continue since could not found notification by id: " +
                        notificationId);

                    return;
                }

                var users = await GetUsersAsync(notificationInfo);

                var userNotifications = await SaveUserNotificationsAsync(users, notificationInfo);

                await NotificationStore.DeleteNotificationAsync(notificationInfo.Id);

                await NotifyAsync(userNotifications.ToArray());

                await uow.CompleteAsync();
            }
        }

        protected virtual async Task<UserIdentifier[]> GetUsersAsync(NotificationSimpleInfo notificationInfo)
        {
            List<UserIdentifier> userIds;

            if (!notificationInfo.UserIds.IsNullOrEmpty())
            {
                // Directly get from UserIds
                userIds = await notificationInfo
                    .UserIds
                    .Split(",")
                    .Select(UserIdentifier.Parse)
                    .ToAsyncEnumerable()
                    .WhereAwait(async uid =>
                        await SettingStore.GetSettingValueForUserAsync<bool>(NotificationSettingNames.ReceiveNotifications, uid.UserId))
                    .ToListAsync();
            }
            else
            {
                var tenantIds = GetTenantIds(notificationInfo);

                List<UserNotificationSubscriptionInfo> subscriptions;

                if (tenantIds.IsNullOrEmpty() ||
                    (tenantIds.Length == 1 && tenantIds[0] == NotificationConsts.AllTenantIds.To<Guid>()))
                {
                    // Get all subscribed users of all tenants
                    subscriptions = await NotificationStore.GetSubscriptionsAsync(
                        notificationInfo.NotificationName,
                        notificationInfo.EntityTypeName,
                        notificationInfo.EntityId,
                        notificationInfo.TargetNotifiersList);
                }
                else
                {
                    // Get all subscribed users of specified tenant(s)
                    subscriptions = await NotificationStore.GetSubscriptionsAsync(
                        tenantIds,
                        notificationInfo.NotificationName,
                        notificationInfo.EntityTypeName,
                        notificationInfo.EntityId,
                        notificationInfo.TargetNotifiersList);
                }

                // Remove invalid subscriptions
                var invalidSubscriptions = new Dictionary<Guid, UserNotificationSubscriptionInfo>();

                // TODO: Group subscriptions per tenant for potential performance improvement
                foreach (var subscription in subscriptions)
                {
                    using (CurrentTenant.Change(subscription.TenantId))
                    {
                        if (!await NotificationDefinitionManager.IsAvailableAsync(notificationInfo.NotificationName, new UserIdentifier(subscription.TenantId, subscription.UserId)) ||
                            !await SettingStore.GetSettingValueForUserAsync<bool>(NotificationSettingNames.ReceiveNotifications, subscription.UserId))
                        {
                            invalidSubscriptions[subscription.Id] = subscription;
                        }
                    }
                }

                subscriptions.RemoveAll(s => invalidSubscriptions.ContainsKey(s.Id));

                // Get user ids
                userIds = subscriptions
                    .Select(s => new UserIdentifier(s.TenantId, s.UserId))
                    .ToList();
            }

            if (!notificationInfo.ExcludedUserIds.IsNullOrEmpty())
            {
                // Exclude specified users.
                var excludedUserIds = notificationInfo
                    .ExcludedUserIds
                    .Split(",")
                    .Select(uidAsStr => UserIdentifier.Parse(uidAsStr))
                    .ToList();

                userIds.RemoveAll(uid => excludedUserIds.Any(euid => euid.Equals(uid)));
            }

            return userIds.ToArray();
        }

        private static Guid?[] GetTenantIds(NotificationSimpleInfo notificationInfo)
        {
            if (notificationInfo.TenantIds.IsNullOrEmpty())
            {
                return null;
            }

            return notificationInfo
                .TenantIds
                .Split(",")
                .Select(tenantIdAsStr => tenantIdAsStr == "null" ? null : (Guid?)tenantIdAsStr.To<Guid>())
                .ToArray();
        }

        #region Protected methods

        protected virtual async Task<List<UserNotificationInfo>> SaveUserNotificationsAsync(UserIdentifier[] users, NotificationSimpleInfo notificationInfo)
        {
            var userNotifications = new List<UserNotificationInfo>();

            var tenantGroups = users.GroupBy(user => user.TenantId);
            foreach (var tenantGroup in tenantGroups)
            {
                using (CurrentTenant.Change(tenantGroup.Key))
                {
                    var publishedNotification = await NotificationStore.InsertPublishedNotificationAsync(
                        tenantGroup.Key,
                        notificationInfo.NotificationName,
                        notificationInfo.Data,
                        notificationInfo.DataTypeName,
                        notificationInfo.EntityTypeName,
                        notificationInfo.EntityTypeAssemblyQualifiedName,
                        notificationInfo.EntityId,
                        notificationInfo.Severity,
                        notificationInfo.CreationTime,
                        notificationInfo.CreatorId,
                        Clock.Now);

                    var userNotificationSubscriptions = await NotificationStore.GetSubscriptionsAsync(
                        notificationInfo.NotificationName,
                        notificationInfo.EntityTypeName,
                        notificationInfo.EntityId,
                        null);

                    foreach (var user in tenantGroup)
                    {
                        var userNotification = await NotificationStore.InsertUserNotificationAsync(publishedNotification.Id, user, notificationInfo.TargetNotifiers, userNotificationSubscriptions, publishedNotification, tenantGroup.Key);
                        userNotifications.Add(userNotification);
                    }
                }
            }

            return userNotifications;
        }

        protected virtual string GetTargetNotifiersForUser(
            UserIdentifier user,
            NotificationInfo notificationInfo,
            List<UserNotificationSubscription> userNotificationSubscriptions)
        {
            if (userNotificationSubscriptions.IsNullOrEmpty())
            {
                return notificationInfo.TargetNotifiers;
            }

            var userSubscription = userNotificationSubscriptions.FirstOrDefault(un => un.UserId == user.UserId);
            if (userSubscription == null)
            {
                return notificationInfo.TargetNotifiers;
            }

            return userSubscription.TargetNotifiers;
        }

        protected virtual async Task NotifyAsync(UserNotificationInfo[] userNotifications)
        {
            foreach (var notifier in RealTimeNotifierManager.Notifiers)
            {
                try
                {
                    UserNotificationInfo[] notificationsToSendWithThatNotifier;

                    // if UseOnlyIfRequestedAsTarget is true, then we should send notifications which requests this notifier
                    if (notifier.UseOnlyIfRequestedAsTarget)
                    {
                        notificationsToSendWithThatNotifier = userNotifications
                            .Where(n => n.TargetNotifiersList.Contains(notifier.Name))
                            .ToArray();
                    }
                    else
                    {
                        // notifier allows to send any notifications
                        // we can send all notifications which does not have TargetNotifiersList(since there is no target, we can send it with any notifier)
                        // or current notifier is in TargetNotifiersList
                        notificationsToSendWithThatNotifier = userNotifications
                            .Where(n =>
                                    n.TargetNotifiersList == null ||
                                    n.TargetNotifiersList.Count ==
                                    0 || // if there is no target notifiers, send it to all of them
                                    n.TargetNotifiersList.Contains(notifier.Name)) // if there is target notifiers, check if current notifier is in it
                            .ToArray();
                    }

                    if (notificationsToSendWithThatNotifier.Length == 0 && !notifier.Name.Equals(NullRealTimeNotifier.NotifierName, StringComparison.Ordinal))
                    {
                        continue;
                    }

                    await notifier.SendNotificationsAsync(notificationsToSendWithThatNotifier);
                }
                catch (Exception ex)
                {
                    Logger.LogWarning(ex.ToString(), ex);
                }
            }
        }

        #endregion
    }
}
