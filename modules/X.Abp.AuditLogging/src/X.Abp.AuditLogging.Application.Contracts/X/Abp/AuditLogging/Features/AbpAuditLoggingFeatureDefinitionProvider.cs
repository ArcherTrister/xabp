// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.AuditLogging.Localization;
using Volo.Abp.Features;
using Volo.Abp.Localization;
using Volo.Abp.Validation.StringValues;

namespace X.Abp.AuditLogging.Features;

public class AbpAuditLoggingFeatureDefinitionProvider : FeatureDefinitionProvider
{
    public override void Define(IFeatureDefinitionContext context)
    {
        var group = context.AddGroup(AbpAuditLoggingFeatures.GroupName,
            L("Feature:AuditLoggingGroup"));

        group.AddFeature(AbpAuditLoggingFeatures.Enable,
            "true",
            L("Feature:AuditLoggingEnable"),
            L("Feature:AuditLoggingEnableDescription"),
            new ToggleStringValueType());
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<AuditLoggingResource>(name);
    }
}
