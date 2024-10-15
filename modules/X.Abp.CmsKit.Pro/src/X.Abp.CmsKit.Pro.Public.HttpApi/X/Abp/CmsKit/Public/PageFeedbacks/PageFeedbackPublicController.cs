// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp;
using Volo.Abp.Features;
using Volo.Abp.GlobalFeatures;

using X.Abp.CmsKit.Features;
using X.Abp.CmsKit.GlobalFeatures;

namespace X.Abp.CmsKit.Public.PageFeedbacks
{
    [RequiresGlobalFeature(typeof(PageFeedbackFeature))]
    [Route("api/cms-kit-public/page-feedback")]
    [RemoteService(true, Name = AbpCmsKitProPublicRemoteServiceConsts.RemoteServiceName)]
    [Area(AbpCmsKitProPublicRemoteServiceConsts.ModuleName)]
    [RequiresFeature(CmsKitProFeatures.PageFeedbackEnable)]
    public class PageFeedbackPublicController : CmsKitProPublicController, IPageFeedbackPublicAppService
    {
        protected virtual IPageFeedbackPublicAppService PageFeedbackPublicAppService { get; }

        public PageFeedbackPublicController(IPageFeedbackPublicAppService pageFeedbackPublicAppService)
        {
            PageFeedbackPublicAppService = pageFeedbackPublicAppService;
        }

        [HttpPost]
        public Task<PageFeedbackDto> CreateAsync(CreatePageFeedbackInput input)
        {
            return PageFeedbackPublicAppService.CreateAsync(input);
        }
    }
}
