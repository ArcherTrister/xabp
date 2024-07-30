// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Asp.Versioning;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Volo.Abp;
using Volo.Abp.Application.Dtos;

using X.Abp.Notification.Dtos;

namespace X.Abp.Notification;

[RemoteService(Name = AbpNotificationRemoteServiceConsts.RemoteServiceName)]
[Area(AbpNotificationRemoteServiceConsts.ModuleName)]
[ControllerName("Notifications")]
[Route("api/notification/user-notification")]
[Authorize]
public class UserNotificationController : NotificationControllerBase, IUserNotificationAppService
{
    protected IUserNotificationAppService UserNotificationAppService { get; }

    public UserNotificationController(IUserNotificationAppService notificationAppService)
    {
        UserNotificationAppService = notificationAppService;
    }

    /// <summary>
    /// Delete a user notification by given id.
    /// </summary>
    /// <param name="id">The user notification id.</param>
    [HttpDelete]
    [Route("{id}")]
    public virtual async Task DeleteAsync(Guid id)
    {
        await UserNotificationAppService.DeleteAsync(id);
    }

    /// <summary>
    /// Gets a user notification by given id.
    /// </summary>
    /// <param name="id">The user notification id.</param>
    [HttpGet]
    [Route("{id}")]
    public virtual async Task<UserNotificationInfo> GetAsync(Guid id)
    {
        return await UserNotificationAppService.GetAsync(id);
    }

    /// <summary>
    /// Gets notifications for a user.
    /// </summary>
    /// <param name="input"></param>
    [HttpGet]
    public virtual async Task<PagedResultDto<UserNotificationInfo>> GetListAsync(PagedUserNotificationDto input)
    {
        return await UserNotificationAppService.GetListAsync(input);
    }

    [HttpPut]
    public virtual async Task MarkAllAsReadAsync()
    {
        await UserNotificationAppService.MarkAllAsReadAsync();
    }

    [HttpPut]
    [Route("{id}")]
    public virtual async Task MarkAsReadedAsync(Guid id)
    {
        await UserNotificationAppService.MarkAsReadedAsync(id);
    }
}
