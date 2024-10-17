// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Threading.Tasks;

using Volo.Abp.Features;
using Volo.Abp.GlobalFeatures;

using X.Abp.CmsKit.Faqs;
using X.Abp.CmsKit.Features;
using X.Abp.CmsKit.GlobalFeatures;

namespace X.Abp.CmsKit.Public.Faqs
{
    [RequiresFeature(CmsKitProFeatures.FaqEnable)]
    [RequiresGlobalFeature(typeof(FaqFeature))]
    public class FaqSectionPublicAppService : PublicAppService, IFaqSectionPublicAppService
    {
        protected IFaqSectionRepository FaqSectionRepository { get; }

        public FaqSectionPublicAppService(IFaqSectionRepository faqSectionRepository)
        {
            FaqSectionRepository = faqSectionRepository;
        }

        public async Task<List<FaqSectionWithQuestionsDto>> GetListSectionWithQuestionsAsync(FaqSectionListFilterInput input)
        {
            List<FaqSectionWithQuestions> source = await FaqSectionRepository.GetListSectionWithQuestionAsync(input.GroupName, input.SectionName);
            return ObjectMapper.Map<List<FaqSectionWithQuestions>, List<FaqSectionWithQuestionsDto>>(source);
        }
    }
}
