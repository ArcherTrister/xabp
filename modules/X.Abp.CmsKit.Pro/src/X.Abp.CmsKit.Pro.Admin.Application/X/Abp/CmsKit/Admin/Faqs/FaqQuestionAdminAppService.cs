// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Features;
using Volo.Abp.GlobalFeatures;

using X.Abp.CmsKit.Admin.Permissions;
using X.Abp.CmsKit.Faqs;
using X.Abp.CmsKit.Features;
using X.Abp.CmsKit.GlobalFeatures;

namespace X.Abp.CmsKit.Admin.Faqs
{
    [Authorize(AbpCmsKitProAdminPermissions.Faqs.Default)]
    [RequiresFeature(CmsKitProFeatures.FaqEnable)]
    [RequiresGlobalFeature(FaqFeature.Name)]
    public class FaqQuestionAdminAppService : CmsKitProAdminAppServiceBase, IFaqQuestionAdminAppService
    {
        protected IFaqQuestionRepository FaqQuestionRepository { get; }

        protected FaqQuestionManager FaqQuestionManager { get; }

        public FaqQuestionAdminAppService(
          IFaqQuestionRepository faqQuestionRepository,
          FaqQuestionManager faqQuestionManager)
        {
            FaqQuestionRepository = faqQuestionRepository;
            FaqQuestionManager = faqQuestionManager;
        }

        public virtual async Task<FaqQuestionDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<FaqQuestion, FaqQuestionDto>(await FaqQuestionRepository.GetAsync(id));
        }

        public virtual async Task<PagedResultDto<FaqQuestionDto>> GetListAsync(
          FaqQuestionListFilterDto input)
        {
            List<FaqQuestion> source = await FaqQuestionRepository.GetListAsync(input.SectionId, input.Filter, input.Sorting, input.SkipCount, input.MaxResultCount);
            return new PagedResultDto<FaqQuestionDto>
            {
                Items = new List<FaqQuestionDto>(ObjectMapper.Map<List<FaqQuestion>, List<FaqQuestionDto>>(source)),
                TotalCount = await FaqQuestionRepository.GetCountAsync(input.SectionId, input.Filter)
            };
        }

        [Authorize(AbpCmsKitProAdminPermissions.Faqs.Create)]
        public virtual async Task<FaqQuestionDto> CreateAsync(CreateFaqQuestionDto input)
        {
            FaqQuestion entity = await FaqQuestionManager.CreateAsync(input.SectionId, input.Title, input.Text, input.Order);

            FaqQuestion source = await FaqQuestionRepository.InsertAsync(entity);

            return ObjectMapper.Map<FaqQuestion, FaqQuestionDto>(source);
        }

        [Authorize(AbpCmsKitProAdminPermissions.Faqs.Update)]
        public virtual async Task<FaqQuestionDto> UpdateAsync(Guid id, UpdateFaqQuestionDto input)
        {
            FaqQuestion question = await FaqQuestionRepository.GetAsync(id);
            await FaqQuestionManager.UpdateTitle(question, input.Title);
            question.SetText(input.Text);
            question.Order = input.Order;
            FaqQuestion source = await FaqQuestionRepository.UpdateAsync(question);
            FaqQuestionDto faqQuestionDto = ObjectMapper.Map<FaqQuestion, FaqQuestionDto>(source);
            return faqQuestionDto;
        }

        [Authorize(AbpCmsKitProAdminPermissions.Faqs.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await FaqQuestionRepository.DeleteAsync(id, false);
        }
    }
}
