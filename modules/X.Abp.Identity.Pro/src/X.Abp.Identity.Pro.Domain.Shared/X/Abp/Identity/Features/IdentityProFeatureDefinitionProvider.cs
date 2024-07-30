// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Features;
using Volo.Abp.Identity.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Validation.StringValues;

namespace X.Abp.Identity.Features;

public class IdentityProFeatureDefinitionProvider : FeatureDefinitionProvider
{
    public override void Define(IFeatureDefinitionContext context)
    {
        var group = context.AddGroup(IdentityProFeature.GroupName, L("Feature:IdentityGroup"));

        group.AddFeature(IdentityProFeature.TwoFactor,
            IdentityProTwoFactorBehaviour.Optional.ToString(),
            L("Feature:TwoFactor"),
            L("Feature:TwoFactorDescription"),
            new SelectionStringValueType
            {
                ItemSource = new StaticSelectionStringValueItemSource(
                    new LocalizableSelectionStringValueItem
                    {
                        Value = IdentityProTwoFactorBehaviour.Optional.ToString(),
                        DisplayText = GetTwoFactorBehaviourLocalizableStringInfo("Feature:TwoFactor.Optional")
                    },
                    new LocalizableSelectionStringValueItem
                    {
                        Value = IdentityProTwoFactorBehaviour.Disabled.ToString(),
                        DisplayText = GetTwoFactorBehaviourLocalizableStringInfo("Feature:TwoFactor.Disabled")
                    },
                    new LocalizableSelectionStringValueItem
                    {
                        Value = IdentityProTwoFactorBehaviour.Forced.ToString(),
                        DisplayText = GetTwoFactorBehaviourLocalizableStringInfo("Feature:TwoFactor.Forced")
                    })
            });

        group.AddFeature(IdentityProFeature.MaxUserCount,
            "0",
            L("Feature:MaximumUserCount"),
            L("Feature:MaximumUserCountDescription"),
            new FreeTextStringValueType(new NumericValueValidator(0)));

        group.AddFeature(IdentityProFeature.EnableLdapLogin,
            false.ToString(),
            L("Feature:EnableLdapLogin"),
            null,
            new ToggleStringValueType());

        group.AddFeature(IdentityProFeature.EnableOAuthLogin,
            false.ToString(),
            L("Feature:EnableOAuthLogin"),
            null,
            new ToggleStringValueType());
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<IdentityResource>(name);
    }

    private static LocalizableStringInfo GetTwoFactorBehaviourLocalizableStringInfo(string key)
    {
        return new LocalizableStringInfo(LocalizationResourceNameAttribute.GetName(typeof(IdentityResource)), key);
    }
}
