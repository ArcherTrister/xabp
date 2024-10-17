// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp;
using Volo.Abp.Features;
using Volo.Abp.GlobalFeatures;

using X.Abp.CmsKit.Features;
using X.Abp.CmsKit.GlobalFeatures;

namespace X.Abp.CmsKit.Public.Faqs
{
    [Route("api/cms-kit-public/faq-section")]
    [RequiresGlobalFeature(typeof(FaqFeature))]
    [Area(AbpCmsKitProPublicRemoteServiceConsts.ModuleName)]
    [RemoteService(true, Name = AbpCmsKitProPublicRemoteServiceConsts.RemoteServiceName)]
    [RequiresFeature(CmsKitProFeatures.FaqEnable)]
    public class FaqSectionPublicController : CmsKitProPublicController, IFaqSectionPublicAppService
    {
        protected IFaqSectionPublicAppService FaqSectionPublicAppService { get; }

        public FaqSectionPublicController(
          IFaqSectionPublicAppService faqSectionPublicAppService)
        {
            FaqSectionPublicAppService = faqSectionPublicAppService;
        }

        [HttpGet]
        public Task<List<FaqSectionWithQuestionsDto>> GetListSectionWithQuestionsAsync(FaqSectionListFilterInput input)
        {
            return FaqSectionPublicAppService.GetListSectionWithQuestionsAsync(input);
        }
    }
}
