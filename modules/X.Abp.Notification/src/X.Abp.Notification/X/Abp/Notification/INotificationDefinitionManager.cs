// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Threading.Tasks;

using JetBrains.Annotations;

namespace X.Abp.Notification;

public interface INotificationDefinitionManager
{
    /// <summary>
    /// Gets a notification definition by name.
    /// Throws exception if there is no notification definition with given name.
    /// </summary>
    [NotNull]
    Task<NotificationDefinition> GetAsync([NotNull] string name);

    /// <summary>
    /// Gets a notification definition by name.
    /// Returns null if there is no notification definition with given name.
    /// </summary>
    Task<NotificationDefinition> GetOrNullAsync(string name);

    /// <summary>
    /// Gets all notification definitions.
    /// </summary>
    Task<IReadOnlyList<NotificationDefinition>> GetAllAsync();

    Task<IReadOnlyList<NotificationGroupDefinition>> GetGroupsAsync();

    /// <summary>
    /// Checks if given notification (<paramref name="name"/>) is available for given user.
    /// </summary>
    Task<bool> IsAvailableAsync(string name, UserIdentifier user);

    /// <summary>
    /// Gets all available notification definitions for given user.
    /// </summary>
    /// <param name="user">User.</param>
    Task<IReadOnlyList<NotificationDefinition>> GetAllAvailableAsync(UserIdentifier user);
}
