// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Volo.Abp.DependencyInjection;
using Volo.Abp.Json;

namespace X.Abp.Notification
{
    /// <summary>
    /// Implements <see cref="IUserNotificationSubscriptionManager"/>.
    /// </summary>
    public class UserNotificationSubscriptionManager : IUserNotificationSubscriptionManager, ITransientDependency
    {
        protected IJsonSerializer JsonSerializer { get; }

        protected INotificationStore NotificationStore { get; }

        protected INotificationDefinitionManager NotificationDefinitionManager { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserNotificationSubscriptionManager"/> class.
        /// </summary>
        public UserNotificationSubscriptionManager(
            IJsonSerializer jsonSerializer,
            INotificationStore notificationStore,
            INotificationDefinitionManager notificationDefinitionManager)
        {
            JsonSerializer = jsonSerializer;
            NotificationStore = notificationStore;
            NotificationDefinitionManager = notificationDefinitionManager;
        }

        public async Task SubscribeAsync(
            UserIdentifier user,
            string notificationName,
            EntityIdentifier entityIdentifier = null,
            string[] targetNotifiers = null)
        {
            if (await IsSubscribedAsync(user, notificationName, entityIdentifier, targetNotifiers))
            {
                return;
            }

            await NotificationStore.InsertSubscriptionAsync(user.TenantId, user.UserId, notificationName, entityIdentifier, targetNotifiers);
        }

        public async Task SubscribeToAllAvailableNotificationsAsync(UserIdentifier user)
        {
            var notificationDefinitions = await NotificationDefinitionManager
                    .GetAllAvailableAsync(user);

            foreach (var notificationDefinition in notificationDefinitions)
            {
                await SubscribeAsync(user, notificationDefinition.Name);
            }
        }

        public async Task UnSubscribeAsync(UserIdentifier user, string notificationName, EntityIdentifier entityIdentifier = null)
        {
            await NotificationStore.DeleteSubscriptionAsync(
                user,
                notificationName,
                entityIdentifier == null ? null : entityIdentifier.Type.FullName,
                entityIdentifier == null ? null : JsonSerializer.Serialize(entityIdentifier.Id));
        }

        // TODO: Can work only for single database approach!
        public async Task<List<UserNotificationSubscriptionInfo>> GetSubscriptionsAsync(string notificationName, EntityIdentifier entityIdentifier = null, string[] targetNotifiers = null)
        {
            return await NotificationStore.GetSubscriptionsAsync(
                notificationName,
                entityIdentifier == null ? null : entityIdentifier.Type.FullName,
                entityIdentifier == null ? null : JsonSerializer.Serialize(entityIdentifier.Id),
                targetNotifiers);
        }

        public async Task<List<UserNotificationSubscriptionInfo>> GetSubscriptionsAsync(
            Guid? tenantId,
            string notificationName,
            EntityIdentifier entityIdentifier = null,
            string[] targetNotifiers = null)
        {
            return await NotificationStore.GetSubscriptionsAsync(
                new[] { tenantId },
                notificationName,
                entityIdentifier == null ? null : entityIdentifier.Type.FullName,
                entityIdentifier == null ? null : JsonSerializer.Serialize(entityIdentifier.Id),
                targetNotifiers);
        }

        public async Task<List<UserNotificationSubscriptionInfo>> GetSubscribedNotificationsAsync(UserIdentifier user)
        {
            return await NotificationStore.GetSubscriptionsAsync(user);
        }

        public Task<bool> IsSubscribedAsync(
            UserIdentifier user,
            string notificationName,
            EntityIdentifier entityIdentifier = null,
            string[] targetNotifiers = null)
        {
            return NotificationStore.IsSubscribedAsync(
                user,
                notificationName,
                entityIdentifier == null ? null : entityIdentifier.Type.FullName,
                entityIdentifier == null ? null : JsonSerializer.Serialize(entityIdentifier.Id),
                targetNotifiers);
        }
    }
}
