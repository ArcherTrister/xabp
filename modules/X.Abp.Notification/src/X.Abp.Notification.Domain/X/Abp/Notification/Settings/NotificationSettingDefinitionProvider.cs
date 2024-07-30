// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Localization;
using Volo.Abp.Settings;

using X.Abp.Notification.Localization;

namespace X.Abp.Notification.Settings;

public class NotificationSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        /* Define module settings here.
         * Use names from NotificationSettings class.
         */
        context.Add(
            new SettingDefinition(
                NotificationSettingNames.ReceiveNotifications,
                true.ToString(),
                L("DisplayName:ReceiveNotifications"),
                L("Description:ReceiveNotifications"),
                true)
            .WithProviders(UserSettingValueProvider.ProviderName));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<AbpNotificationResource>(name);
    }
}
