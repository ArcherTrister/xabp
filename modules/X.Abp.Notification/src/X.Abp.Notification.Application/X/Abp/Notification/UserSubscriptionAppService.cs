// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

using Volo.Abp.Features;
using Volo.Abp.Users;

using X.Abp.Notification.Dtos;
using X.Abp.Notification.Features;

namespace X.Abp.Notification;

[RequiresFeature(NotificationFeatures.Enable)]
[Authorize]
public class UserSubscriptionAppService : NotificationAppServiceBase, IUserSubscriptionAppService
{
    protected IUserNotificationSubscriptionManager NotificationSubscriptionManager => LazyServiceProvider.LazyGetRequiredService<IUserNotificationSubscriptionManager>();

    /// <summary>
    /// Subscribes to a notification for given user and notification informations.
    /// </summary>
    /// <param name="input"></param>
    public virtual async Task SubscribeAsync(SubscribeInput input)
    {
        await NotificationSubscriptionManager.SubscribeAsync(new UserIdentifier(CurrentTenant.Id, CurrentUser.GetId()), input.NotificationName, input.EntityIdentifier, input.TargetNotifiers);
    }

    /// <summary>
    /// Subscribes to all available notifications for given user.
    /// It does not subscribe entity related notifications.
    /// </summary>
    /// <param name="user">User.</param>
    public virtual async Task SubscribeToAllAvailableNotificationsAsync(UserIdentifier user)
    {
        await NotificationSubscriptionManager.SubscribeToAllAvailableNotificationsAsync(new UserIdentifier(CurrentTenant.Id, CurrentUser.GetId()));
    }

    /// <summary>
    /// Unsubscribes from a notification.
    /// </summary>
    /// <param name="input"></param>
    public virtual async Task UnSubscribeAsync(UnsubscribeInput input)
    {
        await NotificationSubscriptionManager.UnSubscribeAsync(new UserIdentifier(CurrentTenant.Id, CurrentUser.GetId()), input.NotificationName, input.EntityIdentifier);
    }

    /// <summary>
    /// Gets subscribed notifications for a user.
    /// </summary>
    /// <param name="user">User.</param>
    public virtual async Task<List<UserNotificationSubscriptionInfo>> GetSubscribedNotificationsAsync(UserIdentifier user)
    {
        return await NotificationSubscriptionManager.GetSubscribedNotificationsAsync(new UserIdentifier(CurrentTenant.Id, CurrentUser.GetId()));
    }

    /// <summary>
    /// Checks if a user subscribed for a notification.
    /// </summary>
    /// <param name="input"></param>
    public virtual async Task<bool> IsSubscribedAsync(SubscribeInput input)
    {
        return await NotificationSubscriptionManager.IsSubscribedAsync(new UserIdentifier(CurrentTenant.Id, CurrentUser.GetId()), input.NotificationName, input.EntityIdentifier, input.TargetNotifiers);
    }
}
