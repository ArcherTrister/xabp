// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Features;

using X.Abp.Notification.Dtos;
using X.Abp.Notification.Features;
using X.Abp.Notification.RealTime;

namespace X.Abp.Notification;

[RequiresFeature(NotificationFeatures.Enable)]
[Authorize]
public class NotificationAppService : NotificationAppServiceBase, INotificationAppService
{
    protected INotificationPublisher NotificationPublisher => LazyServiceProvider.LazyGetRequiredService<INotificationPublisher>();

    protected IRealTimeNotifierManager RealTimeNotifierManager => LazyServiceProvider.LazyGetRequiredService<IRealTimeNotifierManager>();

    protected INotificationDefinitionManager NotificationDefinitionManager => LazyServiceProvider.LazyGetRequiredService<INotificationDefinitionManager>();

    protected IRepository<NotificationInfo, Guid> NotificationInfoRepository => LazyServiceProvider.LazyGetRequiredService<IRepository<NotificationInfo, Guid>>();

    public virtual async Task<GetNotificationListResultDto> GetAsync()
    {
        var result = new GetNotificationListResultDto
        {
            Groups = new List<NotificationGroupDto>()
        };

        foreach (var group in await NotificationDefinitionManager.GetGroupsAsync())
        {
            var groupDto = CreateNotificationGroupDto(group);

            foreach (var featureDefinition in group.GetNotificationsWithChildren())
            {
                if (CurrentTenant.Id == null && !featureDefinition.IsAvailableToHost)
                {
                    continue;
                }

                groupDto.Notifications.Add(CreateNotificationDto(featureDefinition));
            }

            SetNotificationDepth(groupDto.Notifications);

            if (groupDto.Notifications.Count != 0)
            {
                result.Groups.Add(groupDto);
            }
        }

        return result;
    }

    private NotificationGroupDto CreateNotificationGroupDto(NotificationGroupDefinition groupDefinition)
    {
        return new NotificationGroupDto
        {
            Name = groupDefinition.Name,
            DisplayName = groupDefinition.DisplayName?.Localize(StringLocalizerFactory),
            Notifications = new List<NotificationDto>()
        };
    }

    private NotificationDto CreateNotificationDto(NotificationDefinition featureDefinition)
    {
        return new NotificationDto
        {
            Name = featureDefinition.Name,
            DisplayName = featureDefinition.DisplayName?.Localize(StringLocalizerFactory),
            Description = featureDefinition.Description?.Localize(StringLocalizerFactory),
            ParentName = featureDefinition.Parent?.Name
        };
    }

    protected virtual void SetNotificationDepth(List<NotificationDto> features, NotificationDto parentNotification = null, int depth = 0)
    {
        foreach (var feature in features)
        {
            if ((parentNotification == null && feature.ParentName == null) || (parentNotification != null && parentNotification.Name == feature.ParentName))
            {
                feature.Depth = depth;
                SetNotificationDepth(features, feature, depth + 1);
            }
        }
    }

    public virtual async Task<List<NameValue>> GetAssignableNotificationsAsync()
    {
        List<NameValue> result = new List<NameValue>();

        foreach (var notificationDefinition in await NotificationDefinitionManager.GetAllAsync())
        {
            result.Add(new NameValue
            {
                Name = notificationDefinition.DisplayName?.Localize(StringLocalizerFactory),
                Value = notificationDefinition.Name,
            });
        }

        return await Task.FromResult(result);
    }

    public virtual async Task<List<NameValue>> GetAssignableNotifiersAsync()
    {
        List<NameValue> result = new List<NameValue>();

        foreach (var notifier in RealTimeNotifierManager.Notifiers)
        {
            result.Add(new NameValue
            {
                Name = notifier.DisplayName?.Localize(StringLocalizerFactory),
                Value = notifier.Name
            });
        }

        return await Task.FromResult(result);
    }

    public virtual async Task SendByCurrentTenantAsync(SendNotificationByCurrentTenantInput input)
    {
        await NotificationPublisher.PublishAsync(input.NotificationName, input.Data, null, input.Severity, null, null, null, input.TargetNotifiers);
    }

    public virtual async Task SendByTenantsAsync(SendNotificationByTenantsInput input)
    {
        await NotificationPublisher.PublishAsync(input.NotificationName, input.Data, null, input.Severity, null, null, input.TenantIds, input.TargetNotifiers);
    }

    public virtual async Task SendByUsersAsync(SendNotificationByUsersInput input)
    {
        await NotificationPublisher.PublishAsync(input.NotificationName, input.Data, null, input.Severity, input.UserIds, null, null, input.TargetNotifiers);
    }

    public virtual async Task SendByExcludedUsersAsync(SendNotificationByExcludedUsersInput input)
    {
        await NotificationPublisher.PublishAsync(input.NotificationName, input.Data, null, input.Severity, null, input.ExcludedUserIds, null, input.TargetNotifiers);
    }

    public virtual async Task<List<UnPublishedNotificationOutput>> GetUnPublishedNotificationsAsync()
    {
        var notificationInfoQueryable = await NotificationInfoRepository.GetQueryableAsync();

        return notificationInfoQueryable.Select(x => new UnPublishedNotificationOutput
        {
            Id = x.Id,
            Data = x.Data,
            Severity = x.Severity,
            NotificationName = x.NotificationName,
            DataTypeName = x.DataTypeName,
            CreationTime = x.CreationTime
        }).ToList();
    }
}
