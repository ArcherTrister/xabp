// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Volo.Abp.DependencyInjection;
using Volo.Abp.Emailing;
using Volo.Abp.TextTemplating;

using X.Abp.CmsKit.Templates;

namespace X.Abp.CmsKit.PageFeedbacks
{
    public class PageFeedbackEmailSender : ITransientDependency
    {
        protected IEmailSender EmailSender
        { get; }

        protected ITemplateRenderer TemplateRenderer { get; }

        protected ILogger<PageFeedbackEmailSender> Logger { get; }

        public PageFeedbackEmailSender(
          IEmailSender emailSender,
          ITemplateRenderer templateRenderer,
          ILogger<PageFeedbackEmailSender> logger)
        {
            EmailSender = emailSender;
            TemplateRenderer = templateRenderer;
            Logger = logger;
        }

        public virtual async Task QueueAsync(PageFeedback pageFeedback, params string[] emailAddresses)
        {
            // TODO: SEND EMAIL
            string body = await TemplateRenderer.RenderAsync(CmsKitEmailTemplates.PageFeedbackEmailTemplate, new
            {
                Title = "", // Localizer["PageFeedback"],
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
                    //            throw new ArgumentNullException(Localizer["EmailToException"]);
                    //Logger.LogWarning(Class12.smethod_19() + pageFeedback.EntityType);
                }
                else
                {
                    //await EmailSender.QueueAsync(emailAddress.Trim(), Class12.smethod_19(), body);
                }
            }
        }
    }
}
