// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;

using Volo.Abp;
using Volo.Abp.Localization;

namespace X.Abp.Notification;

public class NotificationDefinitionContext : INotificationDefinitionContext
{
    internal Dictionary<string, NotificationGroupDefinition> Groups { get; }

    public NotificationDefinitionContext()
    {
        Groups = new Dictionary<string, NotificationGroupDefinition>();
    }

    public NotificationGroupDefinition AddGroup(string name, ILocalizableString displayName = null)
    {
        Check.NotNull(name, nameof(name));

        if (Groups.TryGetValue(name, out var _))
        {
            throw new AbpException($"There is already an existing notification group with name: {name}");
        }

        return Groups[name] = new NotificationGroupDefinition(name, displayName);
    }

    public NotificationGroupDefinition GetGroupOrNull(string name)
    {
        Check.NotNull(name, nameof(name));

        if (!Groups.TryGetValue(name, out var _))
        {
            return null;
        }

        return Groups[name];
    }

    public void RemoveGroup(string name)
    {
        Check.NotNull(name, nameof(name));

        if (!Groups.TryGetValue(name, out var _))
        {
            throw new AbpException($"Undefined notification group: '{name}'.");
        }

        Groups.Remove(name);
    }
}
