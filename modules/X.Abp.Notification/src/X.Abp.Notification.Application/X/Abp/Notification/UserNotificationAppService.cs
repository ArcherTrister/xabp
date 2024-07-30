// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Features;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Users;

using X.Abp.Notification.Dtos;
using X.Abp.Notification.Features;

namespace X.Abp.Notification;

[RequiresFeature(NotificationFeatures.Enable)]
[Authorize]
public class UserNotificationAppService : NotificationAppServiceBase, IUserNotificationAppService
{
    protected IUserNotificationManager UserNotificationManager => LazyServiceProvider.LazyGetRequiredService<IUserNotificationManager>();

    protected IRepository<PublishedNotification, Guid> PublishedNotificationRepository => LazyServiceProvider.LazyGetRequiredService<IRepository<PublishedNotification, Guid>>();

    protected IRepository<UserNotification, Guid> UserNotificationRepository => LazyServiceProvider.LazyGetRequiredService<IRepository<UserNotification, Guid>>();

    /// <summary>
    /// Delete a user notification by given id.
    /// </summary>
    /// <param name="id">The user notification id.</param>
    public virtual async Task DeleteAsync(Guid id)
    {
        await UserNotificationManager.DeleteUserNotificationAsync(CurrentTenant.Id, id);
    }

    /// <summary>
    /// Gets a user notification by given id.
    /// </summary>
    /// <param name="id">The user notification id.</param>
    public virtual async Task<UserNotificationInfo> GetAsync(Guid id)
    {
        return await UserNotificationManager.GetUserNotificationAsync(CurrentTenant.Id, id);
    }

    /// <summary>
    /// Gets notifications for a user.
    /// </summary>
    /// <param name="input"></param>
    public virtual async Task<PagedResultDto<UserNotificationInfo>> GetListAsync(PagedUserNotificationDto input)
    {
        /*
        var userNotificationQueryable = await UserNotificationInfoRepository.GetQueryableAsync();
        var publishedNotificationQueryable = await PublishedNotificationInfoRepository.GetQueryableAsync();

        var query = from userNotification in userNotificationQueryable
                    .Where(p => p.UserId == CurrentUser.GetId())
                    .WhereIf(input.StartCreationTime.HasValue, x => x.CreationTime >= input.StartCreationTime.Value)
                    .WhereIf(input.EndCreationTime.HasValue, x => x.CreationTime <= input.EndCreationTime.Value)
                    .WhereIf(input.State.HasValue, x => x.State == input.State.Value)
                    join publishedNotification in publishedNotificationQueryable.WhereIf(!input.NotificationName.IsNullOrWhiteSpace(), x => x.NotificationName == input.NotificationName)
                    on userNotification.PublishedNotificationId equals publishedNotification.Id
                    select new UserNotificationDto
                    {
                        Id = userNotification.Id,
                        PublishedNotificationId = userNotification.PublishedNotificationId,
                        State = userNotification.State,
                        TargetNotifiers = userNotification.TargetNotifiers,
                        CreationTime = userNotification.CreationTime,
                        NotificationName = publishedNotification.NotificationName,
                        Data = publishedNotification.Data,
                        DataTypeName = publishedNotification.DataTypeName,
                        Severity = publishedNotification.Severity
                    };

        var totalCount = await AsyncExecuter.LongCountAsync(query);

        query = query.OrderBy(input.Sorting.IsNullOrWhiteSpace() ? nameof(UserNotificationDto.CreationTime) : input.Sorting).PageBy(input);

        var entityDtos = await AsyncExecuter.ToListAsync(query);

        return new PagedResultDto<UserNotificationDto>(totalCount, entityDtos);
        */

        UserIdentifier user = new UserIdentifier(CurrentTenant.Id, CurrentUser.GetId());
        var totalCount = await UserNotificationManager.GetUserNotificationCountAsync(user, input.State, input.NotificationName, input.StartDate, input.EndDate);
        var result = await UserNotificationManager.GetUserNotificationsAsync(user, input.State, input.NotificationName, input.SkipCount, input.MaxResultCount, input.StartDate, input.EndDate);

        return new PagedResultDto<UserNotificationInfo>(totalCount, result);
    }

    public virtual async Task MarkAllAsReadAsync()
    {
        await UserNotificationManager.UpdateAllUserNotificationStatesAsync(new UserIdentifier(CurrentTenant.Id, CurrentUser.GetId()), UserNotificationState.Read);
    }

    public virtual async Task MarkAsReadedAsync(Guid id)
    {
        await UserNotificationManager.UpdateUserNotificationStateAsync(CurrentTenant.Id, id, UserNotificationState.Read);
    }
}
