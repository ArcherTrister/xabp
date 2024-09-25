// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace X.Abp.Notification;

public class StaticNotificationDefinitionStore : IStaticNotificationDefinitionStore, ISingletonDependency
{
    protected IDictionary<string, NotificationGroupDefinition> NotificationGroupDefinitions => _lazyNotificationGroupDefinitions.Value;

    private readonly Lazy<Dictionary<string, NotificationGroupDefinition>> _lazyNotificationGroupDefinitions;

    protected IDictionary<string, NotificationDefinition> NotificationDefinitions => _lazyNotificationDefinitions.Value;

    private readonly Lazy<Dictionary<string, NotificationDefinition>> _lazyNotificationDefinitions;

    protected AbpNotificationOptions Options { get; }

    private readonly IServiceProvider _serviceProvider;

    public StaticNotificationDefinitionStore(
        IOptions<AbpNotificationOptions> options,
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        Options = options.Value;

        _lazyNotificationDefinitions = new Lazy<Dictionary<string, NotificationDefinition>>(
            CreateNotificationDefinitions,
            isThreadSafe: true);

        _lazyNotificationGroupDefinitions = new Lazy<Dictionary<string, NotificationGroupDefinition>>(
            CreateNotificationGroupDefinitions,
            isThreadSafe: true);
    }

    public virtual async Task<NotificationDefinition> GetAsync(string name)
    {
        Check.NotNull(name, nameof(name));

        var notification = await GetOrNullAsync(name);

        if (notification == null)
        {
            throw new AbpException("Undefined notification: " + name);
        }

        return notification;
    }

    protected virtual Dictionary<string, NotificationDefinition> CreateNotificationDefinitions()
    {
        var notifications = new Dictionary<string, NotificationDefinition>();

        foreach (var groupDefinition in NotificationGroupDefinitions.Values)
        {
            foreach (var notification in groupDefinition.Notifications)
            {
                AddNotificationToDictionaryRecursively(notifications, notification);
            }
        }

        return notifications;
    }

    protected virtual void AddNotificationToDictionaryRecursively(
        Dictionary<string, NotificationDefinition> notifications,
        NotificationDefinition notification)
    {
        if (notifications.TryGetValue(notification.Name, out var _))
        {
            throw new AbpException("Duplicate notification name: " + notification.Name);
        }

        notifications[notification.Name] = notification;

        foreach (var child in notification.Children)
        {
            AddNotificationToDictionaryRecursively(notifications, child);
        }
    }

    protected virtual Dictionary<string, NotificationGroupDefinition> CreateNotificationGroupDefinitions()
    {
        var context = new NotificationDefinitionContext();

        using (var scope = _serviceProvider.CreateScope())
        {
            var providers = Options
                .DefinitionProviders
                .Select(p => scope.ServiceProvider.GetRequiredService(p) as INotificationDefinitionProvider)
                .ToList();

            foreach (var provider in providers)
            {
                provider.Define(context);
            }
        }

        return context.Groups;
    }

    public virtual Task<NotificationDefinition> GetOrNullAsync(string name)
    {
        return Task.FromResult(NotificationDefinitions.GetOrDefault(name));
    }

    public virtual Task<IReadOnlyList<NotificationDefinition>> GetNotificationsAsync()
    {
        return Task.FromResult<IReadOnlyList<NotificationDefinition>>(NotificationDefinitions.Values.ToList());
    }

    public virtual Task<IReadOnlyList<NotificationGroupDefinition>> GetGroupsAsync()
    {
        return Task.FromResult<IReadOnlyList<NotificationGroupDefinition>>(NotificationGroupDefinitions.Values.ToList());
    }
}
