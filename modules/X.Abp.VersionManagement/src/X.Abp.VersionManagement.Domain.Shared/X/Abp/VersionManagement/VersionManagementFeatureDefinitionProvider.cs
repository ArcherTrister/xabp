// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Features;
using Volo.Abp.Localization;
using Volo.Abp.Validation.StringValues;

using X.Abp.VersionManagement.Localization;

namespace X.Abp.VersionManagement;

public class VersionManagementFeatureDefinitionProvider : FeatureDefinitionProvider
{
    public override void Define(IFeatureDefinitionContext context)
    {
        var group = context.AddGroup(VersionManagementFeatures.GroupName,
            L("Feature:VersionManagementGroup"));
        group.AddFeature(VersionManagementFeatures.Enable,
            "true",
            L("Feature:VersionManagementEnable"),
            L("Feature:VersionManagementEnableDescription"),
            new ToggleStringValueType());
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<VersionManagementResource>(name);
    }
}
