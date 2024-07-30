// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Features;
using Volo.Abp.Localization;
using Volo.Abp.Validation.StringValues;

using X.Abp.Forms.Localization;

namespace X.Abp.Forms;

public class FormsFeatureDefinitionProvider : FeatureDefinitionProvider
{
    public override void Define(IFeatureDefinitionContext context)
    {
        var group = context.AddGroup(FormsFeatures.GroupName,
            L("Feature:FormsGroup"));

        group.AddFeature(FormsFeatures.Enable,
            "true",
            L("Feature:FormsEnable"),
            L("Feature:FormsEnableDescription"),
            new ToggleStringValueType());
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<FormsResource>(name);
    }
}
