// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Collections.Immutable;

using Volo.Abp;
using Volo.Abp.Localization;

namespace X.Abp.Notification;

public class NotificationGroupDefinition : ICanCreateChildNotification
{
    /// <summary>
    /// Unique name of the group.
    /// </summary>
    public string Name { get; }

    public Dictionary<string, object> Properties { get; }

    public ILocalizableString DisplayName
    {
        get => _displayName;
        set => _displayName = Check.NotNull(value, nameof(value));
    }

    private ILocalizableString _displayName;

    public IReadOnlyList<NotificationDefinition> Notifications => _notifications.ToImmutableList();

    private readonly List<NotificationDefinition> _notifications;

    /// <summary>
    /// Gets/sets a key-value on the <see cref="Properties"/>.
    /// </summary>
    /// <param name="name">Name of the property</param>
    /// <returns>
    /// Returns the value in the <see cref="Properties"/> dictionary by given <paramref name="name"/>.
    /// Returns null if given <paramref name="name"/> is not present in the <see cref="Properties"/> dictionary.
    /// </returns>
    public object this[string name]
    {
        get => Properties.GetOrDefault(name);
        set => Properties[name] = value;
    }

    protected internal NotificationGroupDefinition(
        string name,
        ILocalizableString displayName = null)
    {
        Name = name;
        DisplayName = displayName ?? new FixedLocalizableString(Name);

        Properties = new Dictionary<string, object>();
        _notifications = new List<NotificationDefinition>();
    }

    public virtual NotificationDefinition AddNotification(
        string name,
        ILocalizableString displayName = null,
        ILocalizableString description = null,
        bool isVisibleToClients = true,
        bool isAvailableToHost = true)
    {
        var notification = new NotificationDefinition(
            name,
            displayName,
            description,
            isVisibleToClients,
            isAvailableToHost);

        _notifications.Add(notification);

        return notification;
    }

    public NotificationDefinition CreateChildNotification(string name,
        ILocalizableString displayName = null,
        ILocalizableString description = null,
        bool isVisibleToClients = true,
        bool isAvailableToHost = true)
    {
        return AddNotification(name, displayName, description, isVisibleToClients, isAvailableToHost);
    }

    public virtual List<NotificationDefinition> GetNotificationsWithChildren()
    {
        var notifications = new List<NotificationDefinition>();

        foreach (var notification in _notifications)
        {
            AddNotificationToListRecursively(notifications, notification);
        }

        return notifications;
    }

    /// <summary>
    /// Sets a property in the <see cref="Properties"/> dictionary.
    /// This is a shortcut for nested calls on this object.
    /// </summary>
    public virtual NotificationGroupDefinition WithProperty(string key, object value)
    {
        Properties[key] = value;
        return this;
    }

    private void AddNotificationToListRecursively(List<NotificationDefinition> notifications, NotificationDefinition notification)
    {
        notifications.Add(notification);

        foreach (var child in notification.Children)
        {
            AddNotificationToListRecursively(notifications, child);
        }
    }

    public override string ToString()
    {
        return $"[{nameof(NotificationGroupDefinition)} {Name}]";
    }
}
