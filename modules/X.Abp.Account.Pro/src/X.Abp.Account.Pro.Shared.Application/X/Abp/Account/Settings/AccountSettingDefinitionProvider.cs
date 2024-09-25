// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Localization;
using Volo.Abp.Settings;

using X.Abp.Account.Localization;

namespace X.Abp.Account.Settings;

public class AccountSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        context.Add(
            new SettingDefinition(AccountSettingNames.IsSelfRegistrationEnabled,
            "true",
            L("DisplayName:IsSelfRegistrationEnabled"),
            L("Description:IsSelfRegistrationEnabled"),
            true),
            new SettingDefinition(AccountSettingNames.EnableLocalLogin,
            "true",
            L("DisplayName:EnableLocalLogin"),
            L("Description:EnableLocalLogin"),
            true),
            new SettingDefinition(AccountSettingNames.PreventEmailEnumeration,
            "true",
            L("DisplayName:PreventEmailEnumeration"),
            L("Description:PreventEmailEnumeration"),
            true),
            new SettingDefinition(AccountSettingNames.TwoFactorLogin.IsRememberBrowserEnabled,
            "true",
            L("DisplayName:IsRememberBrowserEnabled"),
            L("Description:IsRememberBrowserEnabled"),
            true),
            new SettingDefinition(AccountSettingNames.Captcha.UseCaptchaOnLogin,
            "false",
            L("DisplayName:UseCaptchaOnLogin"),
            L("Description:UseCaptchaOnLogin"),
            true),
            new SettingDefinition(AccountSettingNames.Captcha.UseCaptchaOnRegistration,
            "false",
            L("DisplayName:UseCaptchaOnRegistration"),
            L("Description:UseCaptchaOnRegistration"),
            true),
            new SettingDefinition(AccountSettingNames.Captcha.VerifyBaseUrl,
            "https://www.google.com/",
            L("DisplayName:VerifyBaseUrl"),
            L("Description:VerifyBaseUrl"),
            true),
            new SettingDefinition(AccountSettingNames.Captcha.SiteKey,
            null,
            L("DisplayName:SiteKey"),
            L("Description:SiteKey"),
            true),
            new SettingDefinition(AccountSettingNames.Captcha.SiteSecret,
            null,
            L("DisplayName:SiteSecret"),
            L("Description:SiteSecret"),
            false),
            new SettingDefinition(AccountSettingNames.Captcha.Version,
            "3",
            L("DisplayName:Version"),
            L("Description:Version"),
            true),
            new SettingDefinition(AccountSettingNames.Captcha.Score,
            "0.5",
            L("DisplayName:Score"),
            L("Description:Score"),
            true),
            new SettingDefinition(AccountSettingNames.ProfilePictureSource,
            false.ToString(),
            L("DisplayName:UseGravatar"),
            L("Description:UseGravatar"),
            true),
            new SettingDefinition(AccountSettingNames.ExternalProviders, isVisibleToClients: false));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<AccountResource>(name);
    }
}
