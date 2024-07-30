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
[Route("api/notification")]
[Authorize]
public class NotificationController : NotificationControllerBase, INotificationAppService
{
    protected INotificationAppService NotificationAppService { get; }

    public NotificationController(INotificationAppService notificationAppService)
    {
        NotificationAppService = notificationAppService;
    }

    [HttpGet]
    public virtual async Task<GetNotificationListResultDto> GetAsync()
    {
        return await NotificationAppService.GetAsync();
    }

    [HttpGet]
    [Route("assignable-notifications")]
    public virtual async Task<List<NameValue>> GetAssignableNotificationsAsync()
    {
        return await NotificationAppService.GetAssignableNotificationsAsync();
    }

    [HttpGet]
    [Route("assignable-notifiers")]
    public virtual async Task<List<NameValue>> GetAssignableNotifiersAsync()
    {
        return await NotificationAppService.GetAssignableNotifiersAsync();
    }

    [HttpPost]
    [Route("send-by-current-tenant")]
    public virtual async Task SendByCurrentTenantAsync(SendNotificationByCurrentTenantInput input)
    {
        await NotificationAppService.SendByCurrentTenantAsync(input);
    }

    [HttpPost]
    [Route("send-by-tenants")]
    public virtual async Task SendByTenantsAsync(SendNotificationByTenantsInput input)
    {
        await NotificationAppService.SendByTenantsAsync(input);
    }

    [HttpPost]
    [Route("send-by-users")]
    public virtual async Task SendByUsersAsync(SendNotificationByUsersInput input)
    {
        await NotificationAppService.SendByUsersAsync(input);
    }

    [HttpPost]
    [Route("send-by-excluded-users")]
    public virtual async Task SendByExcludedUsersAsync(SendNotificationByExcludedUsersInput input)
    {
        await NotificationAppService.SendByExcludedUsersAsync(input);
    }

    [HttpGet]
    [Route("un-published-notifications")]
    public virtual async Task<List<UnPublishedNotificationOutput>> GetUnPublishedNotificationsAsync()
    {
        return await NotificationAppService.GetUnPublishedNotificationsAsync();
    }
}
