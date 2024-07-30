// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Emailing.Templates;
using Volo.Abp.Localization;
using Volo.Abp.TextTemplating;

using X.Abp.Account.Localization;

namespace X.Abp.Account.Emailing.Templates;

public class AccountEmailTemplateDefinitionProvider : TemplateDefinitionProvider
{
    public override void Define(ITemplateDefinitionContext context)
    {
        context.Add(
            new TemplateDefinition(
                AccountEmailTemplates.PasswordResetLink,
                displayName: L($"TextTemplate:{AccountEmailTemplates.PasswordResetLink}"),
                layout: StandardEmailTemplates.Layout,
                localizationResource: typeof(AccountResource))
            .WithVirtualFilePath("/X/Abp/Account/Emailing/Templates/PasswordResetLink.tpl", true));

        context.Add(
            new TemplateDefinition(
                AccountEmailTemplates.EmailConfirmationLink,
                displayName: L($"TextTemplate:{AccountEmailTemplates.EmailConfirmationLink}"),
                layout: StandardEmailTemplates.Layout,
                localizationResource: typeof(AccountResource))
            .WithVirtualFilePath("/X/Abp/Account/Emailing/Templates/EmailConfirmationLink.tpl", true));

        context.Add(
            new TemplateDefinition(
                AccountEmailTemplates.EmailSecurityCode,
                displayName: L($"TextTemplate:{AccountEmailTemplates.EmailSecurityCode}"),
                layout: StandardEmailTemplates.Layout,
                localizationResource: typeof(AccountResource))
            .WithVirtualFilePath("/X/Abp/Account/Emailing/Templates/EmailSecurityCode.tpl", true));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<AccountResource>(name);
    }
}
