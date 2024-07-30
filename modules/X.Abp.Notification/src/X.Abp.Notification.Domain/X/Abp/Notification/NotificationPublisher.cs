// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace X.Abp.Notification;

/// <summary>
/// Implements <see cref="INotificationPublisher"/>.
/// </summary>
public class NotificationPublisher : INotificationPublisher, ITransientDependency
{
    public const int MaxUserCountToDirectlyDistributeANotification = 5;

    protected INotificationStore NotificationStore { get; }

    protected INotificationDistributer NotificationDistributer { get; }

    protected ICurrentTenant CurrentTenant { get; }

    protected IBackgroundJobManager BackgroundJobManager { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationPublisher"/> class.
    /// </summary>
    public NotificationPublisher(
        INotificationStore notificationStore,
        INotificationDistributer notificationDistributer,
        ICurrentTenant currentTenant,
        IBackgroundJobManager backgroundJobManager)
    {
        NotificationStore = notificationStore;
        NotificationDistributer = notificationDistributer;
        CurrentTenant = currentTenant;
        BackgroundJobManager = backgroundJobManager;
    }

    public virtual async Task PublishAsync(
            string notificationName,
            NotificationData data = null,
            EntityIdentifier entityIdentifier = null,
            NotificationSeverity severity = NotificationSeverity.Info,
            UserIdentifier[] userIds = null,
            UserIdentifier[] excludedUserIds = null,
            Guid?[] tenantIds = null,
            string[] targetNotifiers = null)
    {
        if (notificationName.IsNullOrEmpty())
        {
            throw new ArgumentException("NotificationName can not be null or whitespace!", nameof(notificationName));
        }

        if (!tenantIds.IsNullOrEmpty() && !userIds.IsNullOrEmpty())
        {
            throw new ArgumentException("tenantIds can be set only if userIds is not set!", nameof(tenantIds));
        }

        if (tenantIds.IsNullOrEmpty() && userIds.IsNullOrEmpty())
        {
            tenantIds = new[] { CurrentTenant.Id };
        }

        var notificationInfoId = await NotificationStore.InsertNotificationAsync(notificationName, data, entityIdentifier, severity, userIds, excludedUserIds, tenantIds, targetNotifiers);

        if (userIds != null && userIds.Length <= MaxUserCountToDirectlyDistributeANotification)
        {
            // We can directly distribute the notification since there are not much receivers
            await NotificationDistributer.DistributeAsync(notificationInfoId);
        }
        else
        {
            // We enqueue a background job since distributing may get a long time
            await BackgroundJobManager.EnqueueAsync(new NotificationDistributionJobArgs(notificationInfoId));
        }
    }
}
