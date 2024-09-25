// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using JetBrains.Annotations;

using Volo.Abp.Localization;

namespace X.Abp.Notification;

public interface INotificationDefinitionContext
{
    NotificationGroupDefinition AddGroup([NotNull] string name, ILocalizableString displayName = null);

    NotificationGroupDefinition GetGroupOrNull(string name);

    void RemoveGroup(string name);
}
