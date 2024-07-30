// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Localization;

namespace X.Abp.Notification;

public interface ICanCreateChildNotification
{
    NotificationDefinition CreateChildNotification(
        string name,
        ILocalizableString displayName = null,
        ILocalizableString description = null,
        bool isVisibleToClients = true,
        bool isAvailableToHost = true);
}
