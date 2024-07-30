// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Features;
using Volo.Abp.Localization;
using Volo.Abp.Validation.StringValues;

using X.Abp.LeptonTheme.Management.Localization;

namespace X.Abp.LeptonTheme.Management.Features
{
    public class LeptonThemeManagementFeatureDefinitionProvider : FeatureDefinitionProvider
    {
        public override void Define(IFeatureDefinitionContext context)
        {
            var group = context.AddGroup(LeptonThemeManagementFeatures.GroupName,
                L("Feature:LeptonThemeManagementGroup"));

            group.AddFeature(LeptonThemeManagementFeatures.Enable,
                "true",
                L("Feature:LeptonThemeManagementEnable"),
                L("Feature:LeptonThemeManagementEnableDescription"),
                new ToggleStringValueType());
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<LeptonThemeManagementResource>(name);
        }
    }
}
