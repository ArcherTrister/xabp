// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace X.Abp.Notification;

public class NotificationDefinitionManager : INotificationDefinitionManager, ISingletonDependency
{
    protected IStaticNotificationDefinitionStore StaticStore { get; }

    protected IDynamicNotificationDefinitionStore DynamicStore { get; }

    public NotificationDefinitionManager(
        IStaticNotificationDefinitionStore staticStore,
        IDynamicNotificationDefinitionStore dynamicStore)
    {
        StaticStore = staticStore;
        DynamicStore = dynamicStore;
    }

    public virtual async Task<NotificationDefinition> GetAsync(string name)
    {
        var notification = await GetOrNullAsync(name);
        if (notification == null)
        {
            throw new AbpException("Undefined notification: " + name);
        }

        return notification;
    }

    public virtual async Task<NotificationDefinition> GetOrNullAsync(string name)
    {
        Check.NotNull(name, nameof(name));

        return await StaticStore.GetOrNullAsync(name) ??
               await DynamicStore.GetOrNullAsync(name);
    }

    public virtual async Task<IReadOnlyList<NotificationDefinition>> GetAllAsync()
    {
        var staticNotifications = await StaticStore.GetNotificationsAsync();
        var staticNotificationNames = staticNotifications
            .Select(p => p.Name)
            .ToImmutableHashSet();

        var dynamicNotifications = await DynamicStore.GetNotificationsAsync();

        /* We prefer static notifications over dynamics */
        return staticNotifications.Concat(
            dynamicNotifications.Where(d => !staticNotificationNames.Contains(d.Name)))
        .ToImmutableList();
    }

    public virtual async Task<IReadOnlyList<NotificationGroupDefinition>> GetGroupsAsync()
    {
        var staticGroups = await StaticStore.GetGroupsAsync();
        var staticGroupNames = staticGroups
            .Select(p => p.Name)
            .ToImmutableHashSet();

        var dynamicGroups = await DynamicStore.GetGroupsAsync();

        /* We prefer static groups over dynamics */
        return staticGroups.Concat(
            dynamicGroups.Where(d => !staticGroupNames.Contains(d.Name)))
        .ToImmutableList();
    }

    public virtual async Task<bool> IsAvailableAsync(string name, UserIdentifier user)
    {
        // TODO: IsAvailable
        /*
        var notificationDefinition = GetOrNull(name);
        if (notificationDefinition == null)
        {
            return true;
        }

        if (notificationDefinition.NotificationDependency != null)
        {
            using (var notificationDependencyContext = _iocManager.ResolveAsDisposable<NotificationDependencyContext>())
            {
                notificationDependencyContext.Object.TenantId = user.TenantId;

                if (!notificationDefinition.NotificationDependency.IsSatisfied(notificationDependencyContext.Object))
                {
                    return false;
                }
            }
        }

        if (notificationDefinition.PermissionDependency != null)
        {
            using (var permissionDependencyContext = _iocManager.ResolveAsDisposable<PermissionDependencyContext>())
            {
                permissionDependencyContext.Object.User = user;

                if (!notificationDefinition.PermissionDependency.IsSatisfied(permissionDependencyContext.Object))
                {
                    return false;
                }
            }
        }
        */

        return await Task.FromResult(true);
    }

    public virtual async Task<IReadOnlyList<NotificationDefinition>> GetAllAvailableAsync(UserIdentifier user)
    {
        // TODO: GetAllAvailable
        /*
        var availableDefinitions = new List<NotificationDefinition>();

        using (var permissionDependencyContext = _iocManager.ResolveAsDisposable<PermissionDependencyContext>())
        {
            permissionDependencyContext.Object.User = user;

            using (var notificationDependencyContext = _iocManager.ResolveAsDisposable<NotificationDependencyContext>())
            {
                notificationDependencyContext.Object.TenantId = user.TenantId;

                foreach (var notificationDefinition in GetAll())
                {
                    if (notificationDefinition.PermissionDependency != null &&
                        !notificationDefinition.PermissionDependency.IsSatisfied(permissionDependencyContext.Object))
                    {
                        continue;
                    }

                    if (user.TenantId.HasValue &&
                        notificationDefinition.NotificationDependency != null &&
                        !notificationDefinition.NotificationDependency.IsSatisfied(notificationDependencyContext.Object))
                    {
                        continue;
                    }

                    availableDefinitions.Add(notificationDefinition);
                }
            }
        }

        return availableDefinitions.ToImmutableList();
        */

        return (await GetAllAsync()).ToImmutableList();
    }
}
