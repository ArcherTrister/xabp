// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Localization;

using Volo.Abp.DependencyInjection;
using Volo.Abp.Emailing;
using Volo.Abp.SettingManagement;
using Volo.Abp.TextTemplating;
using Volo.CmsKit.Localization;

using X.Abp.CmsKit.Templates;

namespace X.Abp.CmsKit.Contact;

public class ContactEmailSender : ITransientDependency
{
    protected IEmailSender EmailSender { get; }

    protected ITemplateRenderer TemplateRenderer { get; }

    protected IStringLocalizer<CmsKitResource> Localizer { get; }

    protected ISettingManager SettingManager { get; }

    public ContactEmailSender(
      IEmailSender emailSender,
      ITemplateRenderer templateRenderer,
      IStringLocalizer<CmsKitResource> localizer,
      ISettingManager settingManager)
    {
        EmailSender = emailSender;
        TemplateRenderer = templateRenderer;
        Localizer = localizer;
        SettingManager = settingManager;
    }

    public virtual async Task SendAsync(
      string name,
      string subject,
      string email,
      string message)
    {
        var receiverEmail = await SettingManager.GetOrNullForCurrentTenantAsync(CmsKitProSettingNames.Contact.ReceiverEmailAddress, true);
        if (string.IsNullOrWhiteSpace(receiverEmail))
        {
            throw new ArgumentNullException(Localizer["EmailToException"]);
        }

        var body = await TemplateRenderer.RenderAsync(CmsKitEmailTemplates.ContactEmailTemplate,
            new
            {
                Title = Localizer["Contact"],
                Name = string.Format("{0} : {1}", Localizer["Name"], name),
                Email = string.Format("{0} : {1}", Localizer["EmailAddress"], email),
                Message = string.Format("{0} : {1}", Localizer["Message"], message)
            });

        await EmailSender.SendAsync(receiverEmail, subject, body, true);
    }
}
