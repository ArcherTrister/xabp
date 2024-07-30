// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Features;
using Volo.Abp.Localization;
using Volo.Abp.Validation.StringValues;

using X.Abp.LanguageManagement.Localization;

namespace X.Abp.LanguageManagement;

public class LanguageManagementFeatureDefinitionProvider : FeatureDefinitionProvider
{
    public override void Define(IFeatureDefinitionContext context)
    {
        var group = context.AddGroup(LanguageManagementFeatures.GroupName,
            L("Feature:LanguageManagementGroup"));
        group.AddFeature(LanguageManagementFeatures.Enable,
            "true",
            L("Feature:LanguageManagementEnable"),
            L("Feature:LanguageManagementEnableDescription"),
            new ToggleStringValueType());
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<LanguageManagementResource>(name);
    }
}
