// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Features;
using Volo.Abp.Localization;
using Volo.Abp.Validation.StringValues;

using X.Abp.Notification.Localization;

namespace X.Abp.Notification.Features;

public class NotificationFeatureDefinitionProvider : FeatureDefinitionProvider
{
    public override void Define(IFeatureDefinitionContext context)
    {
        var group = context.AddGroup(NotificationFeatures.GroupName, L("Feature:NotificationGroup"));
        group.AddFeature(NotificationFeatures.Enable,
            "true",
            L("Feature:NotificationEnable"),
            L("Feature:NotificationEnableDescription"),
            new ToggleStringValueType());
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<AbpNotificationResource>(name);
    }
}
