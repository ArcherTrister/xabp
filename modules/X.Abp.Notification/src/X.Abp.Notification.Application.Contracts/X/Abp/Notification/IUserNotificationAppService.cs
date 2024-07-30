// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

using X.Abp.Notification.Dtos;

namespace X.Abp.Notification;

public interface IUserNotificationAppService : IApplicationService
{
    /// <summary>
    /// Delete a user notification by given id.
    /// </summary>
    /// <param name="id">The user notification id.</param>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Gets a user notification by given id.
    /// </summary>
    /// <param name="id">The user notification id.</param>
    Task<UserNotificationInfo> GetAsync(Guid id);

    /// <summary>
    /// Gets notifications for a user.
    /// </summary>
    /// <param name="input"></param>
    Task<PagedResultDto<UserNotificationInfo>> GetListAsync(PagedUserNotificationDto input);

    Task MarkAllAsReadAsync();

    Task MarkAsReadedAsync(Guid id);
}
