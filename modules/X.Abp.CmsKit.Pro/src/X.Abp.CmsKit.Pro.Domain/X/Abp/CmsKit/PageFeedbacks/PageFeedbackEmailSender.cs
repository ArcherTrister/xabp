// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

using Volo.Abp.DependencyInjection;
using Volo.Abp.Emailing;
using Volo.Abp.TextTemplating;
using Volo.CmsKit.Localization;

using X.Abp.CmsKit.Templates;

namespace X.Abp.CmsKit.PageFeedbacks
{
    public class PageFeedbackEmailSender : ITransientDependency
    {
        protected IEmailSender EmailSender { get; }

        protected ITemplateRenderer TemplateRenderer { get; }

        protected IStringLocalizer<CmsKitResource> Localizer { get; }

        protected ILogger<PageFeedbackEmailSender> Logger { get; }

        public PageFeedbackEmailSender(
          IEmailSender emailSender,
          ITemplateRenderer templateRenderer,
          IStringLocalizer<CmsKitResource> localizer,
          ILogger<PageFeedbackEmailSender> logger)
        {
            EmailSender = emailSender;
            TemplateRenderer = templateRenderer;
            Localizer = localizer;
            Logger = logger;
        }

        public virtual async Task QueueAsync(PageFeedback pageFeedback, params string[] emailAddresses)
        {
            string body = await TemplateRenderer.RenderAsync(CmsKitEmailTemplates.PageFeedbackEmailTemplate, new
            {
                Title = Localizer["PageFeedback"],
                EntityType = pageFeedback.EntityType,
                Url = pageFeedback.Url,
                IsUseful = pageFeedback.IsUseful,
                UserNote = pageFeedback.UserNote,
                CreationTime = pageFeedback.CreationTime
            });

            for (int index = 0; index < emailAddresses.Length; ++index)
            {
                string emailAddress = emailAddresses[index];
                if (emailAddress.IsNullOrWhiteSpace())
                {
                    Logger.LogWarning("Email address is empty for page feedback setting with entity type: {EntityType}", pageFeedback.EntityType);
                }
                else
                {
                    await EmailSender.QueueAsync(emailAddress.Trim(), Localizer["PageFeedback"], body);
                }
            }
        }
    }
}
