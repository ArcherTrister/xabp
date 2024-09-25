// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Volo.Abp.DependencyInjection;

namespace X.Abp.Notification
{
  /// <summary>
  /// Implements  <see cref="IUserNotificationManager"/>.
  /// </summary>
  public class UserNotificationManager : IUserNotificationManager, ISingletonDependency
  {
    protected INotificationStore NotificationStore { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserNotificationManager"/> class.
    /// </summary>
    public UserNotificationManager(INotificationStore notificationStore)
    {
      NotificationStore = notificationStore;
    }

    public virtual async Task<List<UserNotificationInfo>> GetUserNotificationsAsync(UserIdentifier user, UserNotificationState? state = null, string notificationName = null, int skipCount = 0, int maxResultCount = int.MaxValue, DateTime? startDate = null, DateTime? endDate = null)
    {
      return await NotificationStore.GetUserNotificationsWithNotificationsAsync(user, state, notificationName, skipCount, maxResultCount, startDate, endDate);
    }

    public virtual Task<int> GetUserNotificationCountAsync(UserIdentifier user, UserNotificationState? state = null, string notificationName = null, DateTime? startDate = null, DateTime? endDate = null)
    {
      return NotificationStore.GetUserNotificationCountAsync(user, state, notificationName, startDate, endDate);
    }

    public virtual async Task<UserNotificationInfo> GetUserNotificationAsync(Guid? tenantId, Guid userNotificationId)
    {
      return await NotificationStore.GetUserNotificationWithNotificationOrNullAsync(tenantId, userNotificationId);
    }

    public virtual Task UpdateUserNotificationStateAsync(Guid? tenantId, Guid userNotificationId, UserNotificationState state)
    {
      return NotificationStore.UpdateUserNotificationStateAsync(tenantId, userNotificationId, state);
    }

    public virtual Task UpdateAllUserNotificationStatesAsync(UserIdentifier user, UserNotificationState state)
    {
      return NotificationStore.UpdateAllUserNotificationStatesAsync(user, state);
    }

    public virtual Task DeleteUserNotificationAsync(Guid? tenantId, Guid userNotificationId)
    {
      return NotificationStore.DeleteUserNotificationAsync(tenantId, userNotificationId);
    }

    public virtual Task DeleteAllUserNotificationsAsync(UserIdentifier user, UserNotificationState? state = null, DateTime? startDate = null, DateTime? endDate = null)
    {
      return NotificationStore.DeleteAllUserNotificationsAsync(user, state, startDate, endDate);
    }
  }
}
