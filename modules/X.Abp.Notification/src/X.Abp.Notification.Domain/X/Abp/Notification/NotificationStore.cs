// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Json;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

using X.Abp.Notification.RealTime;

namespace X.Abp.Notification
{
    /// <summary>
    /// Implements <see cref="INotificationStore"/> using repositories.
    /// </summary>
    public class NotificationStore : INotificationStore, ITransientDependency
    {
        protected IRepository<NotificationInfo, Guid> NotificationInfoRepository { get; }

        protected IRepository<PublishedNotification, Guid> PublishedNotificationRepository { get; }

        protected IRepository<UserNotification, Guid> UserNotificationRepository { get; }

        protected IRepository<UserNotificationSubscription, Guid> NotificationSubscriptionRepository { get; }

        protected IRealTimeNotifierManager RealTimeNotifierManager { get; }

        protected IJsonSerializer JsonSerializer { get; }

        protected IGuidGenerator GuidGenerator { get; }

        protected IDataFilter DataFilter { get; }

        protected ICurrentTenant CurrentTenant { get; }

        protected IUnitOfWorkManager UnitOfWorkManager { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationStore"/> class.
        /// </summary>
        public NotificationStore(
            IRepository<NotificationInfo, Guid> notificationInfoRepository,
            IRepository<PublishedNotification, Guid> publishedNotificationRepository,
            IRepository<UserNotification, Guid> userNotificationRepository,
            IRepository<UserNotificationSubscription, Guid> notificationSubscriptionRepository,
            IRealTimeNotifierManager realTimeNotifierManager,
            IJsonSerializer jsonSerializer,
            IGuidGenerator guidGenerator,
            IDataFilter dataFilter,
            ICurrentTenant currentTenant,
            IUnitOfWorkManager unitOfWorkManager)
        {
            NotificationInfoRepository = notificationInfoRepository;
            PublishedNotificationRepository = publishedNotificationRepository;
            UserNotificationRepository = userNotificationRepository;
            NotificationSubscriptionRepository = notificationSubscriptionRepository;
            RealTimeNotifierManager = realTimeNotifierManager;
            JsonSerializer = jsonSerializer;
            GuidGenerator = guidGenerator;
            DataFilter = dataFilter;
            CurrentTenant = currentTenant;
            UnitOfWorkManager = unitOfWorkManager;
        }

        public virtual async Task InsertSubscriptionAsync(
            Guid? tenantId,
            Guid userId,
            string notificationName,
            EntityIdentifier entityIdentifier = null,
            string[] targetNotifiers = null)
        {
            using (var uow = UnitOfWorkManager.Begin(true))
            {
                using (CurrentTenant.Change(tenantId))
                {
                    UserNotificationSubscription subscriptionInfo;
                    if (entityIdentifier == null)
                    {
                        subscriptionInfo = new UserNotificationSubscription(
                            GuidGenerator.Create(),
                            tenantId,
                            userId,
                            notificationName,
                            null,
                            null,
                            null,
                            targetNotifiers);
                    }
                    else
                    {
                        subscriptionInfo = new UserNotificationSubscription(
                            GuidGenerator.Create(),
                            tenantId,
                            userId,
                            notificationName,
                            JsonSerializer.Serialize(entityIdentifier.Id),
                            entityIdentifier.Type.FullName,
                            entityIdentifier.Type.AssemblyQualifiedName,
                            targetNotifiers);
                    }

                    await NotificationSubscriptionRepository.InsertAsync(subscriptionInfo, true);
                    await uow.CompleteAsync();
                }
            }
        }

        public virtual async Task DeleteSubscriptionAsync(
            UserIdentifier user,
            string notificationName,
            string entityTypeName,
            string entityId)
        {
            using (var uow = UnitOfWorkManager.Begin(true))
            {
                using (CurrentTenant.Change(user.TenantId))
                {
                    await NotificationSubscriptionRepository.DeleteAsync(s =>
                        s.UserId == user.UserId &&
                        s.NotificationName == notificationName &&
                        s.EntityTypeName == entityTypeName &&
                        s.EntityId == entityId,
                        true);

                    await uow.CompleteAsync();
                }
            }
        }

        public virtual async Task<Guid> InsertNotificationAsync(
            string notificationName,
            NotificationData data = null,
            EntityIdentifier entityIdentifier = null,
            NotificationSeverity severity = NotificationSeverity.Info,
            UserIdentifier[] userIds = null,
            UserIdentifier[] excludedUserIds = null,
            Guid?[] tenantIds = null,
            string[] targetNotifiers = null)
        {
            using (var uow = UnitOfWorkManager.Begin(true))
            {
                using (CurrentTenant.Change(null))
                {
                    NotificationInfo notificationInfo;
                    if (entityIdentifier == null)
                    {
                        notificationInfo = new NotificationInfo(
                            GuidGenerator.Create(),
                            notificationName,
                            JsonSerializer.Serialize(data),
                            null,
                            null,
                            null,
                            severity,
                            userIds,
                            excludedUserIds,
                            tenantIds,
                            GetTargetNotifiers(targetNotifiers));
                    }
                    else
                    {
                        notificationInfo = new NotificationInfo(
                            GuidGenerator.Create(),
                            notificationName,
                            JsonSerializer.Serialize(data),
                            JsonSerializer.Serialize(entityIdentifier.Id),
                            entityIdentifier.Type.FullName,
                            entityIdentifier.Type.AssemblyQualifiedName,
                            severity,
                            userIds,
                            excludedUserIds,
                            tenantIds,
                            GetTargetNotifiers(targetNotifiers));
                    }

                    await NotificationInfoRepository.InsertAsync(notificationInfo, true);
                    await uow.CompleteAsync();
                    return notificationInfo.Id;
                }
            }
        }

        public virtual async Task<NotificationSimpleInfo> GetNotificationOrNullAsync(Guid notificationId)
        {
            using (CurrentTenant.Change(null))
            {
                var notificationInfo = await NotificationInfoRepository.FirstOrDefaultAsync(p => p.Id == notificationId);
                if (notificationInfo == null)
                {
                    return null;
                }

                return notificationInfo.ToNotificationSimpleInfo();
            }
        }

        public virtual async Task<UserNotificationInfo> InsertUserNotificationAsync(Guid publishedNotificationId, UserIdentifier user, string targetNotifiers, List<UserNotificationSubscriptionInfo> userNotificationSubscriptions, PublishedNotificationInfo publishedNotification, Guid? tenantId)
        {
            using (var uow = UnitOfWorkManager.Begin(true))
            {
                using (CurrentTenant.Change(tenantId))
                {
                    var userNotificationInfo = new UserNotification(GuidGenerator.Create(), publishedNotification.CreationTime)
                    {
                        TenantId = tenantId,
                        UserId = user.UserId,
                        PublishedNotificationId = publishedNotificationId,
                        TargetNotifiers = GetTargetNotifiersForUser(user, targetNotifiers, userNotificationSubscriptions)
                    };
                    await UserNotificationRepository.InsertAsync(userNotificationInfo, true);
                    await uow.CompleteAsync();
                    return userNotificationInfo.ToUserNotificationInfo(publishedNotification);
                }
            }
        }

        public virtual async Task<List<UserNotificationSubscriptionInfo>> GetSubscriptionsAsync(
            string notificationName,
            string entityTypeName,
            string entityId,
            string[] targetNotifiers)
        {
            using (DataFilter.Disable<IMultiTenant>())
            {
                var predicate = GetNotificationSubscriptionPredicate(
                    notificationName,
                    entityTypeName,
                    entityId,
                    targetNotifiers);

                var notificationSubscriptions = await NotificationSubscriptionRepository.GetListAsync(predicate);
                return notificationSubscriptions.Select(p => p.ToUserNotificationSubscriptionInfo(JsonSerializer)).ToList();
            }
        }

        public virtual async Task<List<UserNotificationSubscriptionInfo>> GetSubscriptionsAsync(
            Guid?[] tenantIds,
            string notificationName,
            string entityTypeName,
            string entityId,
            string[] targetNotifiers)
        {
            var subscriptions = new List<UserNotificationSubscriptionInfo>();

            foreach (var tenantId in tenantIds)
            {
                subscriptions.AddRange(
                    await GetSubscriptionsAsync(
                        tenantId,
                        notificationName,
                        entityTypeName,
                        entityId,
                        targetNotifiers));
            }

            return subscriptions;
        }

        public virtual async Task<List<UserNotificationSubscriptionInfo>> GetSubscriptionsAsync(UserIdentifier user)
        {
            using (CurrentTenant.Change(user.TenantId))
            {
                var notificationSubscriptions = await NotificationSubscriptionRepository.GetListAsync(s => s.UserId == user.UserId);
                return notificationSubscriptions.Select(p => p.ToUserNotificationSubscriptionInfo(JsonSerializer)).ToList();
            }
        }

        protected virtual async Task<List<UserNotificationSubscriptionInfo>> GetSubscriptionsAsync(
            Guid? tenantId,
            string notificationName,
            string entityTypeName,
            string entityId,
            string[] targetNotifiers)
        {
            using (CurrentTenant.Change(tenantId))
            {
                var predicate = GetNotificationSubscriptionPredicate(
                    notificationName,
                    entityTypeName,
                    entityId,
                    targetNotifiers);

                var notificationSubscriptions = await NotificationSubscriptionRepository.GetListAsync(predicate);
                return notificationSubscriptions.Select(p => p.ToUserNotificationSubscriptionInfo(JsonSerializer)).ToList();
            }
        }

        protected virtual ExpressionStarter<UserNotificationSubscription> GetNotificationSubscriptionPredicate(
            string notificationName,
            string entityTypeName,
            string entityId,
            string[] targetNotifiers)
        {
            var predicate = PredicateBuilder.New<UserNotificationSubscription>();
            predicate = predicate.And(e => e.NotificationName == notificationName);
            predicate = predicate.And(e => e.EntityTypeName == entityTypeName);
            predicate = predicate.And(e => e.EntityId == entityId);

            if (!targetNotifiers.IsNullOrEmpty())
            {
                var targetNotifierPredicate = PredicateBuilder.New<UserNotificationSubscription>();
                foreach (var targetNotifier in targetNotifiers)
                {
                    targetNotifierPredicate = targetNotifierPredicate.Or(e => e.TargetNotifiers.Contains(targetNotifier));
                }

                predicate = predicate.And(targetNotifierPredicate);
            }

            return predicate;
        }

        public virtual async Task<bool> IsSubscribedAsync(
            UserIdentifier user,
            string notificationName,
            string entityTypeName,
            string entityId,
            string[] targetNotifiers)
        {
            using (CurrentTenant.Change(user.TenantId))
            {
                var predicate = GetNotificationSubscriptionPredicate(
                    notificationName,
                    entityTypeName,
                    entityId,
                    targetNotifiers);

                predicate = predicate.And(e => e.UserId == user.UserId);

                return await NotificationSubscriptionRepository.AnyAsync(predicate);
            }
        }

        public virtual async Task UpdateUserNotificationStateAsync(
            Guid? tenantId,
            Guid userNotificationId,
            UserNotificationState state)
        {
            using (var uow = UnitOfWorkManager.Begin(true))
            {
                using (CurrentTenant.Change(tenantId))
                {
                    var userNotification = await UserNotificationRepository.FirstOrDefaultAsync(p => p.Id == userNotificationId);
                    if (userNotification == null)
                    {
                        return;
                    }

                    userNotification.State = state;
                    await UserNotificationRepository.UpdateAsync(userNotification, true);

                    await uow.CompleteAsync();
                }
            }
        }

        public virtual async Task UpdateAllUserNotificationStatesAsync(UserIdentifier user, UserNotificationState state)
        {
            using (var uow = UnitOfWorkManager.Begin(true))
            {
                using (CurrentTenant.Change(user.TenantId))
                {
                    var userNotifications = await UserNotificationRepository.GetListAsync(
                        un => un.UserId == user.UserId);

                    foreach (var userNotification in userNotifications)
                    {
                        userNotification.State = state;
                    }

                    await UserNotificationRepository.UpdateManyAsync(userNotifications, true);

                    await uow.CompleteAsync();
                }
            }
        }

        public virtual async Task DeleteUserNotificationAsync(Guid? tenantId, Guid userNotificationId)
        {
            using (var uow = UnitOfWorkManager.Begin(true))
            {
                using (CurrentTenant.Change(tenantId))
                {
                    await UserNotificationRepository.DeleteAsync(userNotificationId, true);
                    await uow.CompleteAsync();
                }
            }
        }

        public virtual async Task DeleteAllUserNotificationsAsync(
            UserIdentifier user,
            UserNotificationState? state = null,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            using (var uow = UnitOfWorkManager.Begin(true))
            {
                using (CurrentTenant.Change(user.TenantId))
                {
                    var predicate = NotificationStore.CreateNotificationFilterPredicate(user, state, startDate, endDate);

                    await UserNotificationRepository.DeleteAsync(predicate, true);
                    await uow.CompleteAsync();
                }
            }
        }

        private static Expression<Func<UserNotification, bool>> CreateNotificationFilterPredicate(
            UserIdentifier user,
            UserNotificationState? state = null,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var predicate = PredicateBuilder.New<UserNotification>();
            predicate = predicate.And(p => p.UserId == user.UserId);

            if (state.HasValue)
            {
                predicate = predicate.And(p => p.State == state);
            }

            if (startDate.HasValue)
            {
                predicate = predicate.And(p => p.CreationTime >= startDate);
            }

            if (endDate.HasValue)
            {
                predicate = predicate.And(p => p.CreationTime <= endDate);
            }

            return predicate;
        }

        public virtual async Task<List<UserNotificationInfo>> GetUserNotificationsWithNotificationsAsync(
                UserIdentifier user,
                UserNotificationState? state = null,
                string notificationName = null,
                int skipCount = 0,
                int maxResultCount = int.MaxValue,
                DateTime? startDate = null,
                DateTime? endDate = null)
        {
            using (CurrentTenant.Change(user.TenantId))
            {
                var userNotificationQueryable = await UserNotificationRepository.GetQueryableAsync();
                var publishedNotificationQueryable = await PublishedNotificationRepository.GetQueryableAsync();

                var query = from userNotificationInfo in userNotificationQueryable
                            join publishedNotification in publishedNotificationQueryable on userNotificationInfo
                                .PublishedNotificationId equals publishedNotification.Id
                            where userNotificationInfo.UserId == user.UserId
                            orderby publishedNotification.CreationTime descending
                            select new
                            {
                                userNotificationInfo,
                                publishedNotification
                            };

                query = query
                    .WhereIf(state.HasValue, x => x.userNotificationInfo.State == state.Value)
                    .WhereIf(!notificationName.IsNullOrWhiteSpace(), x => x.publishedNotification.NotificationName == notificationName)
                    .WhereIf(startDate.HasValue, x => x.publishedNotification.CreationTime >= startDate)
                    .WhereIf(endDate.HasValue, x => x.publishedNotification.CreationTime <= endDate);

                query = query.PageBy(skipCount, maxResultCount);

                var list = query.ToList();

                return list.Select(
                    a => a.userNotificationInfo.ToUserNotificationInfo(a.publishedNotification.ToPublishedNotificationInfo(JsonSerializer))).ToList();
            }
        }

        public virtual async Task<int> GetUserNotificationCountAsync(
            UserIdentifier user,
            UserNotificationState? state = null,
            string notificationName = null,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            using (CurrentTenant.Change(user.TenantId))
            {
                /*
                var predicate = CreateNotificationFilterPredicate(user, state, startDate, endDate);
                return await UserNotificationRepository.CountAsync(predicate);
                */
                var userNotificationQueryable = await UserNotificationRepository.GetQueryableAsync();
                var publishedNotificationQueryable = await PublishedNotificationRepository.GetQueryableAsync();

                var query = from userNotificationInfo in userNotificationQueryable
                            join publishedNotification in publishedNotificationQueryable on userNotificationInfo
                                .PublishedNotificationId equals publishedNotification.Id
                            where userNotificationInfo.UserId == user.UserId
                            orderby publishedNotification.CreationTime descending
                            select new
                            {
                                userNotificationInfo,
                                publishedNotification
                            };

                query = query
                    .WhereIf(state.HasValue, x => x.userNotificationInfo.State == state.Value)
                    .WhereIf(!notificationName.IsNullOrWhiteSpace(), x => x.publishedNotification.NotificationName == notificationName)
                    .WhereIf(startDate.HasValue, x => x.publishedNotification.CreationTime >= startDate)
                    .WhereIf(endDate.HasValue, x => x.publishedNotification.CreationTime <= endDate);

                return query.Count();
            }
        }

        public virtual async Task<UserNotificationInfo> GetUserNotificationWithNotificationOrNullAsync(Guid? tenantId, Guid userNotificationId)
        {
            using (CurrentTenant.Change(tenantId))
            {
                var userNotificationQueryable = await UserNotificationRepository.GetQueryableAsync();
                var publishedNotificationQueryable = await PublishedNotificationRepository.GetQueryableAsync();
                var query = from userNotificationInfo in userNotificationQueryable
                            join publishedNotification in publishedNotificationQueryable on userNotificationInfo
                                .PublishedNotificationId equals publishedNotification.Id
                            where userNotificationInfo.Id == userNotificationId
                            select new
                            {
                                userNotificationInfo,
                                publishedNotification
                            };

                var item = query.FirstOrDefault();
                if (item == null)
                {
                    return null;
                }

                return item.userNotificationInfo.ToUserNotificationInfo(item.publishedNotification.ToPublishedNotificationInfo(JsonSerializer));
            }
        }

        public virtual async Task<PublishedNotificationInfo> InsertPublishedNotificationAsync(
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
            DateTime publishedTime)
        {
            using (var uow = UnitOfWorkManager.Begin(true))
            {
                using (CurrentTenant.Change(tenantId))
                {
                    var publishedNotification = new PublishedNotification(GuidGenerator.Create(),
                        tenantId,
                        notificationName,
                        data,
                        dataTypeName,
                        entityTypeName,
                        entityTypeAssemblyQualifiedName,
                        entityId,
                        severity,
                        creationTime,
                        creatorId,
                        publishedTime);

                    await PublishedNotificationRepository.InsertAsync(publishedNotification);
                    await uow.CompleteAsync();
                    return publishedNotification.ToPublishedNotificationInfo(JsonSerializer);
                }
            }
        }

        public virtual async Task DeleteNotificationAsync(Guid notificationId)
        {
            using (var uow = UnitOfWorkManager.Begin(true))
            {
                await NotificationInfoRepository.DeleteAsync(notificationId, true);
                await uow.CompleteAsync();
            }
        }

        /*
        public virtual async Task<List<GetNotificationsCreatedByUserOutput>> GetNotificationsPublishedByUserAsync(
            UserIdentifier user, string notificationName, DateTime? startDate, DateTime? endDate)
        {
            using (CurrentTenant.Change(user.TenantId))
            {
                var notificationQueryable = await NotificationRepository.GetQueryableAsync();
                var queryForNotPublishedNotifications = notificationQueryable
                    .Where(n => n.CreatorId == user.UserId && n.NotificationName == notificationName);

                if (startDate.HasValue)
                {
                    queryForNotPublishedNotifications = queryForNotPublishedNotifications
                        .Where(x => x.CreationTime >= startDate);
                }

                if (endDate.HasValue)
                {
                    queryForNotPublishedNotifications = queryForNotPublishedNotifications
                        .Where(x => x.CreationTime <= endDate);
                }

                var result = new List<GetNotificationsCreatedByUserOutput>();

                var unPublishedNotifications = queryForNotPublishedNotifications.Select(x => new GetNotificationsCreatedByUserOutput
                {
                    Data = x.Data,
                    Severity = x.Severity,
                    NotificationName = x.NotificationName,
                    DataTypeName = x.DataTypeName,
                    IsPublished = false,
                    CreationTime = x.CreationTime
                }).ToList();

                result.AddRange(unPublishedNotifications);

                var publishedNotificationQueryable = await PublishedNotificationRepository.GetQueryableAsync();
                var queryForPublishedNotifications = publishedNotificationQueryable
                    .Where(n => n.CreatorId == user.UserId && n.NotificationName == notificationName);

                if (startDate.HasValue)
                {
                    queryForPublishedNotifications = queryForPublishedNotifications
                        .Where(x => x.CreationTime >= startDate);
                }

                if (endDate.HasValue)
                {
                    queryForPublishedNotifications = queryForPublishedNotifications
                        .Where(x => x.CreationTime <= endDate);
                }

                queryForPublishedNotifications = queryForPublishedNotifications
                    .OrderByDescending(n => n.CreationTime);

                var publishedNotifications = queryForPublishedNotifications.Select(x => new GetNotificationsCreatedByUserOutput
                {
                    Data = x.Data,
                    Severity = x.Severity,
                    NotificationName = x.NotificationName,
                    DataTypeName = x.DataTypeName,
                    IsPublished = true,
                    CreationTime = x.CreationTime
                }).ToList();

                result.AddRange(publishedNotifications);
                return result;
            }
        }
        */

        protected virtual string GetTargetNotifiersForUser(UserIdentifier user, string targetNotifiers, List<UserNotificationSubscriptionInfo> userNotificationSubscriptions)
        {
            if (userNotificationSubscriptions.IsNullOrEmpty())
            {
                return targetNotifiers;
            }

            var userSubscription = userNotificationSubscriptions.FirstOrDefault(un => un.UserId == user.UserId);
            if (userSubscription == null)
            {
                return targetNotifiers;
            }

            return userSubscription.TargetNotifiers;
        }

        protected virtual string[] GetTargetNotifiers(string[] targetNotifiers)
        {
            if (targetNotifiers == null)
            {
                return null;
            }

            var allNotificationNotifiers = RealTimeNotifierManager.Notifiers.Select(notifier => notifier.Name).ToList();

            foreach (var targetNotifier in targetNotifiers)
            {
                if (!allNotificationNotifiers.Contains(targetNotifier))
                {
                    throw new ApplicationException("Given target notifier is not registered before: " + targetNotifier + " You must register it to the AbpNotificationOptions.Notifiers!");
                }
            }

            return targetNotifiers.Select(n => n).ToArray();
        }
    }
}
