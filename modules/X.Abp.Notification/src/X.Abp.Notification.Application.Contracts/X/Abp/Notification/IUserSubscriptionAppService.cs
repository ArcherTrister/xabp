// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Threading.Tasks;

using Volo.Abp.Application.Services;

using X.Abp.Notification.Dtos;

namespace X.Abp.Notification;
public interface IUserSubscriptionAppService : IApplicationService
{
    /// <summary>
    /// Subscribes to a notification for given user and notification informations.
    /// </summary>
    /// <param name="input"></param>
    Task SubscribeAsync(SubscribeInput input);

    /// <summary>
    /// Subscribes to all available notifications for given user.
    /// It does not subscribe entity related notifications.
    /// </summary>
    /// <param name="user">User.</param>
    Task SubscribeToAllAvailableNotificationsAsync(UserIdentifier user);

    /// <summary>
    /// Unsubscribes from a notification.
    /// </summary>
    /// <param name="input"></param>
    Task UnSubscribeAsync(UnsubscribeInput input);

    /// <summary>
    /// Gets subscribed notifications for a user.
    /// </summary>
    /// <param name="user">User.</param>
    Task<List<UserNotificationSubscriptionInfo>> GetSubscribedNotificationsAsync(UserIdentifier user);

    /// <summary>
    /// Checks if a user subscribed for a notification.
    /// </summary>
    /// <param name="input"></param>
    Task<bool> IsSubscribedAsync(SubscribeInput input);
}
