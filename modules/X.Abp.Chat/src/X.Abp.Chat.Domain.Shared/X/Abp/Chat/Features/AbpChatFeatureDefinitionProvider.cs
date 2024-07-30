// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Features;
using Volo.Abp.Localization;
using Volo.Abp.Validation.StringValues;

using X.Abp.Chat.Localization;

namespace X.Abp.Chat.Features;

public class AbpChatFeatureDefinitionProvider : FeatureDefinitionProvider
{
    public override void Define(IFeatureDefinitionContext context)
    {
        var group = context.AddGroup(AbpChatFeatures.GroupName,
            L("Feature:ChatGroup"));
        group.AddFeature(AbpChatFeatures.Enable,
            false.ToString(),
            L("Feature:ChatEnable"),
            L("Feature:ChatEnableDescription"),
            new ToggleStringValueType());
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<AbpChatResource>(name);
    }
}
