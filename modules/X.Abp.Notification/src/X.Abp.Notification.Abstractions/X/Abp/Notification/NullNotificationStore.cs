﻿// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Volo.Abp.DependencyInjection;

namespace X.Abp.Notification;

/// <summary>
/// Null pattern implementation of <see cref="INotificationStore"/>.
/// </summary>
[Dependency(TryRegister = true)]
public class NullNotificationStore : INotificationStore, ISingletonDependency
{
  public virtual Task DeleteAllUserNotificationsAsync(UserIdentifier user, UserNotificationState? state = null, DateTime? startDate = null, DateTime? endDate = null)
  {
    return Task.CompletedTask;
  }

  public virtual Task DeleteNotificationAsync(Guid notificationId)
  {
    return Task.CompletedTask;
  }

  public virtual Task DeleteSubscriptionAsync(UserIdentifier user, string notificationName, string entityTypeName, string entityId)
  {
    return Task.CompletedTask;
  }

  public virtual Task DeleteUserNotificationAsync(Guid? tenantId, Guid userNotificationId)
  {
    return Task.CompletedTask;
  }

  public virtual Task<NotificationSimpleInfo> GetNotificationOrNullAsync(Guid notificationId)
  {
    return Task.FromResult(new NotificationSimpleInfo() { Id = notificationId });
  }

  public virtual Task<List<UserNotificationSubscriptionInfo>> GetSubscriptionsAsync(string notificationName, string entityTypeName, string entityId, string[] targetNotifiers)
  {
    return Task.FromResult(new List<UserNotificationSubscriptionInfo>());
  }

  public virtual Task<List<UserNotificationSubscriptionInfo>> GetSubscriptionsAsync(Guid?[] tenantIds, string notificationName, string entityTypeName, string entityId, string[] targetNotifiers)
  {
    return Task.FromResult(new List<UserNotificationSubscriptionInfo>());
  }

  public virtual Task<List<UserNotificationSubscriptionInfo>> GetSubscriptionsAsync(UserIdentifier user)
  {
    return Task.FromResult(new List<UserNotificationSubscriptionInfo>());
  }

  public virtual Task<int> GetUserNotificationCountAsync(UserIdentifier user, UserNotificationState? state = null, string notificationName = null, DateTime? startDate = null, DateTime? endDate = null)
  {
    return Task.FromResult(0);
  }

  public virtual Task<List<UserNotificationInfo>> GetUserNotificationsWithNotificationsAsync(UserIdentifier user, UserNotificationState? state = null, string notificationName = null, int skipCount = 0, int maxResultCount = int.MaxValue, DateTime? startDate = null, DateTime? endDate = null)
  {
    return Task.FromResult(new List<UserNotificationInfo>());
  }

  public virtual Task<UserNotificationInfo> GetUserNotificationWithNotificationOrNullAsync(Guid? tenantId, Guid userNotificationId)
  {
    return Task.FromResult(new UserNotificationInfo() { Id = userNotificationId });
  }

  public virtual Task<Guid> InsertNotificationAsync(string notificationName, NotificationData data = null, EntityIdentifier entityIdentifier = null, NotificationSeverity severity = NotificationSeverity.Info, UserIdentifier[] userIds = null, UserIdentifier[] excludedUserIds = null, Guid?[] tenantIds = null, string[] targetNotifiers = null)
  {
    return Task.FromResult<Guid>(default);
  }

  public virtual Task InsertSubscriptionAsync(
      Guid? tenantId,
      Guid userId,
      string notificationName,
      EntityIdentifier entityIdentifier = null,
      string[] targetNotifiers = null)
  {
    return Task.CompletedTask;
  }

  public virtual Task<PublishedNotificationInfo> InsertPublishedNotificationAsync(Guid? tenantId, string notificationName, string data, string dataTypeName, string entityTypeName, string entityTypeAssemblyQualifiedName, string entityId, NotificationSeverity severity, DateTime creationTime, Guid? creatorId, DateTime publishedTime)
  {
    return Task.FromResult<PublishedNotificationInfo>(null);
  }

  public virtual Task<UserNotificationInfo> InsertUserNotificationAsync(Guid publishedNotificationId, UserIdentifier user, string targetNotifiers, List<UserNotificationSubscriptionInfo> userNotificationSubscriptions, PublishedNotificationInfo publishedNotification, Guid? tenantId)
  {
    return Task.FromResult<UserNotificationInfo>(null);
  }

  public virtual Task<bool> IsSubscribedAsync(UserIdentifier user, string notificationName, string entityTypeName, string entityId, string[] targetNotifiers)
  {
    return Task.FromResult(true);
  }

  public virtual Task UpdateAllUserNotificationStatesAsync(UserIdentifier user, UserNotificationState state)
  {
    return Task.CompletedTask;
  }

  public virtual Task UpdateUserNotificationStateAsync(Guid? tenantId, Guid userNotificationId, UserNotificationState state)
  {
    return Task.CompletedTask;
  }
}
