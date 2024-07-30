// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Threading.Tasks;

using Volo.Abp;
using Volo.Abp.Application.Services;

using X.Abp.Notification.Dtos;

namespace X.Abp.Notification;
public interface INotificationAppService : IApplicationService
{
    Task<GetNotificationListResultDto> GetAsync();

    Task<List<NameValue>> GetAssignableNotificationsAsync();

    Task<List<NameValue>> GetAssignableNotifiersAsync();

    Task SendByCurrentTenantAsync(SendNotificationByCurrentTenantInput input);

    Task SendByTenantsAsync(SendNotificationByTenantsInput input);

    Task SendByUsersAsync(SendNotificationByUsersInput input);

    Task SendByExcludedUsersAsync(SendNotificationByExcludedUsersInput input);

    Task<List<UnPublishedNotificationOutput>> GetUnPublishedNotificationsAsync();
}
