// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Threading.Tasks;

using Asp.Versioning;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Volo.Abp;

using X.Abp.Notification.Dtos;

namespace X.Abp.Notification;

[RemoteService(Name = AbpNotificationRemoteServiceConsts.RemoteServiceName)]
[Area(AbpNotificationRemoteServiceConsts.ModuleName)]
[ControllerName("Notifications")]
[Route("api/notification/user-subscription")]
[Authorize]
public class UserSubscriptionController : NotificationControllerBase, IUserSubscriptionAppService
{
    protected IUserSubscriptionAppService UserSubscriptionAppService { get; }

    public UserSubscriptionController(IUserSubscriptionAppService userSubscriptionAppService)
    {
        UserSubscriptionAppService = userSubscriptionAppService;
    }

    /// <summary>
    /// Subscribes to a notification for given user and notification informations.
    /// </summary>
    /// <param name="input"></param>
    [HttpPost]
    [Route("subscribe")]
    public virtual async Task SubscribeAsync(SubscribeInput input)
    {
        await UserSubscriptionAppService.SubscribeAsync(input);
    }

    /// <summary>
    /// Subscribes to all available notifications for given user.
    /// It does not subscribe entity related notifications.
    /// </summary>
    /// <param name="user">User.</param>
    [HttpPost]
    [Route("subscribe-to-all-available-notifications")]
    public virtual async Task SubscribeToAllAvailableNotificationsAsync(UserIdentifier user)
    {
        await UserSubscriptionAppService.SubscribeToAllAvailableNotificationsAsync(user);
    }

    /// <summary>
    /// Unsubscribes from a notification.
    /// </summary>
    /// <param name="input"></param>
    [HttpPost]
    [Route("un-subscribe")]
    public virtual async Task UnSubscribeAsync(UnsubscribeInput input)
    {
        await UserSubscriptionAppService.UnSubscribeAsync(input);
    }

    /// <summary>
    /// Gets subscribed notifications for a user.
    /// </summary>
    /// <param name="user">User.</param>
    [HttpGet]
    [Route("subscribed-notifications")]
    public virtual async Task<List<UserNotificationSubscriptionInfo>> GetSubscribedNotificationsAsync(UserIdentifier user)
    {
        return await UserSubscriptionAppService.GetSubscribedNotificationsAsync(user);
    }

    /// <summary>
    /// Checks if a user subscribed for a notification.
    /// </summary>
    /// <param name="input"></param>
    [HttpPost]
    [Route("is-subscribed")]
    public virtual async Task<bool> IsSubscribedAsync(SubscribeInput input)
    {
        return await UserSubscriptionAppService.IsSubscribedAsync(input);
    }
}
