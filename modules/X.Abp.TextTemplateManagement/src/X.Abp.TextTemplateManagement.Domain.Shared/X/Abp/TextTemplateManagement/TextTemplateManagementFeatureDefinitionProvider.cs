// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Features;
using Volo.Abp.Localization;
using Volo.Abp.Validation.StringValues;

using X.Abp.TextTemplateManagement.Localization;

namespace X.Abp.TextTemplateManagement;

public class TextTemplateManagementFeatureDefinitionProvider : FeatureDefinitionProvider
{
    public override void Define(IFeatureDefinitionContext context)
    {
        context.AddGroup(TextTemplateManagementFeatures.GroupName, L("Feature:TextTemplateManagementGroup"))
            .AddFeature(TextTemplateManagementFeatures.Enable, "true", L("Feature:TextTemplateManagementEnable"), L("Feature:TextTemplateManagementEnableDescription"), new ToggleStringValueType());
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<TextTemplateManagementResource>(name);
    }
}
