// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using JetBrains.Annotations;

using Volo.Abp;
using Volo.Abp.Localization;

namespace X.Abp.Notification;

public class NotificationDefinition : ICanCreateChildNotification
{
    /// <summary>
    /// Unique name of the notification.
    /// </summary>
    [NotNull]
    public string Name { get; }

    [NotNull]
    public ILocalizableString DisplayName
    {
        get => _displayName;
        set => _displayName = Check.NotNull(value, nameof(value));
    }

    private ILocalizableString _displayName;

    [CanBeNull]
    public ILocalizableString Description { get; set; }

    /// <summary>
    /// Parent of this notification, if one exists.
    /// If set, this notification can be enabled only if the parent is enabled.
    /// </summary>
    [CanBeNull]
    public NotificationDefinition Parent { get; private set; }

    /// <summary>
    /// List of child notifications.
    /// </summary>
    public IReadOnlyList<NotificationDefinition> Children => _children.ToImmutableList();

    private readonly List<NotificationDefinition> _children;

    /// <summary>
    /// Can clients see this notification and it's value.
    /// Default: true.
    /// </summary>
    public bool IsVisibleToClients { get; set; }

    /// <summary>
    /// Can host use this notification.
    /// Default: true.
    /// </summary>
    public bool IsAvailableToHost { get; set; }

    /// <summary>
    /// Gets/sets a key-value on the <see cref="Properties"/>.
    /// </summary>
    /// <param name="name">Name of the property</param>
    /// <returns>
    /// Returns the value in the <see cref="Properties"/> dictionary by given <paramref name="name"/>.
    /// Returns null if given <paramref name="name"/> is not present in the <see cref="Properties"/> dictionary.
    /// </returns>
    [CanBeNull]
    public object this[string name]
    {
        get => Properties.GetOrDefault(name);
        set => Properties[name] = value;
    }

    /// <summary>
    /// Can be used to get/set custom properties for this notification.
    /// </summary>
    [NotNull]
    public Dictionary<string, object> Properties { get; }

    public NotificationDefinition(
        string name,
        ILocalizableString displayName = null,
        ILocalizableString description = null,
        bool isVisibleToClients = true,
        bool isAvailableToHost = true)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name));
        DisplayName = displayName ?? new FixedLocalizableString(name);
        Description = description;
        IsVisibleToClients = isVisibleToClients;
        IsAvailableToHost = isAvailableToHost;

        Properties = new Dictionary<string, object>();
        _children = new List<NotificationDefinition>();
    }

    /// <summary>
    /// Sets a property in the <see cref="Properties"/> dictionary.
    /// This is a shortcut for nested calls on this object.
    /// </summary>
    public virtual NotificationDefinition WithProperty(string key, object value)
    {
        Properties[key] = value;
        return this;
    }

    /// <summary>
    /// Adds a child notification.
    /// </summary>
    /// <returns>Returns a newly created child notification</returns>
    public NotificationDefinition CreateChild(
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
            isAvailableToHost)
        {
            Parent = this
        };

        _children.Add(notification);
        return notification;
    }

    public void RemoveChild(string name)
    {
        var notificationToRemove = _children.FirstOrDefault(f => f.Name == name);
        if (notificationToRemove == null)
        {
            throw new AbpException($"Could not find a notification named '{name}' in the Children of this notification '{Name}'.");
        }

        notificationToRemove.Parent = null;
        _children.Remove(notificationToRemove);
    }

    public NotificationDefinition CreateChildNotification(string name,
        ILocalizableString displayName = null,
        ILocalizableString description = null,
        bool isVisibleToClients = true,
        bool isAvailableToHost = true)
    {
        return CreateChild(name, displayName, description, isVisibleToClients, isAvailableToHost);
    }

    public override string ToString()
    {
        return $"[{nameof(NotificationDefinition)}: {Name}]";
    }
}
