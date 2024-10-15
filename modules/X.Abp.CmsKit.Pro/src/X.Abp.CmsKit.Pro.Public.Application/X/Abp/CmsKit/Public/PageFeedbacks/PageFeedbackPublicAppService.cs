// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Volo.Abp.Features;
using Volo.Abp.GlobalFeatures;

using X.Abp.CmsKit.Features;
using X.Abp.CmsKit.GlobalFeatures;
using X.Abp.CmsKit.PageFeedbacks;

namespace X.Abp.CmsKit.Public.PageFeedbacks
{
    [RequiresFeature(CmsKitProFeatures.PageFeedbackEnable)]
    [RequiresGlobalFeature(typeof(PageFeedbackFeature))]
    public class PageFeedbackPublicAppService : PublicAppService, IPageFeedbackPublicAppService
    {
        protected virtual IPageFeedbackRepository PageFeedbackRepository { get; }

        protected virtual PageFeedbackManager PageFeedbackManager { get; }

        protected virtual IPageFeedbackSettingRepository PageFeedbackSettingRepository { get; }

        protected virtual PageFeedbackEmailSender PageFeedbackEmailSender { get; }

        public PageFeedbackPublicAppService(
          IPageFeedbackRepository pageFeedbackRepository,
          PageFeedbackManager pageFeedbackManager,
          IPageFeedbackSettingRepository pageFeedbackSettingRepository,
          PageFeedbackEmailSender pageFeedbackEmailSender)
        {
            PageFeedbackRepository = pageFeedbackRepository;
            PageFeedbackManager = pageFeedbackManager;
            PageFeedbackSettingRepository = pageFeedbackSettingRepository;
            PageFeedbackEmailSender = pageFeedbackEmailSender;
        }

        public virtual async Task<PageFeedbackDto> CreateAsync(CreatePageFeedbackInput input)
        {
            PageFeedback pageFeedback = await PageFeedbackRepository.InsertAsync(await PageFeedbackManager.CreateAsync(input.EntityType, input.EntityId, input.Url, input.IsUseful, input.UserNote));
            await SendEmailsAsync(pageFeedback);
            return ObjectMapper.Map<PageFeedback, PageFeedbackDto>(pageFeedback);
        }

        protected virtual async Task SendEmailsAsync(PageFeedback pageFeedback)
        {
            PageFeedbackSetting pageFeedbackSetting = await PageFeedbackSettingRepository.FindByEntityTypeAsync(pageFeedback.EntityType);
            if (pageFeedbackSetting == null || AbpStringExtensions.IsNullOrWhiteSpace(pageFeedbackSetting.EmailAddresses))
            {
                pageFeedbackSetting = await PageFeedbackSettingRepository.FindByEntityTypeAsync(null);
            }

            if (pageFeedbackSetting == null)
            {
                return;
            }

            await PageFeedbackEmailSender.QueueAsync(pageFeedback, pageFeedbackSetting.EmailAddresses.Split(PageFeedbackConst.EmailAddressesSeparator));
        }
    }
}
