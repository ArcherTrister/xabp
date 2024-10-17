// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

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
    public class FaqSectionAdminAppService : CmsKitProAdminAppServiceBase, IFaqSectionAdminAppService
    {
        protected IFaqSectionRepository FaqSectionRepository { get; }

        protected FaqSectionManager FaqSectionManager { get; }

        protected FaqOptions FaqOptions { get; }

        public FaqSectionAdminAppService(
          IFaqSectionRepository faqSectionRepository,
          FaqSectionManager faqSectionManager,
          IOptions<FaqOptions> options)
        {
            FaqSectionRepository = faqSectionRepository;
            FaqSectionManager = faqSectionManager;
            FaqOptions = options.Value;
        }

        public virtual async Task<FaqSectionDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<FaqSection, FaqSectionDto>(await FaqSectionRepository.GetAsync(id));
        }

        public virtual async Task<PagedResultDto<FaqSectionWithQuestionCountDto>> GetListAsync(FaqSectionListFilterDto input)
        {
            List<FaqSectionWithQuestionCount> source = await FaqSectionRepository.GetListAsync(input.Filter, input.Sorting, input.SkipCount, input.MaxResultCount);
            return new PagedResultDto<FaqSectionWithQuestionCountDto>
            {
                Items = new List<FaqSectionWithQuestionCountDto>(ObjectMapper.Map<List<FaqSectionWithQuestionCount>, List<FaqSectionWithQuestionCountDto>>(source)),

                TotalCount = await FaqSectionRepository.GetCountAsync(input.Filter)
            };
        }

        [Authorize(AbpCmsKitProAdminPermissions.Faqs.Create)]
        public virtual async Task<FaqSectionDto> CreateAsync(CreateFaqSectionDto input)
        {
            FaqSection section = await FaqSectionManager.CreateAsync(input.GroupName, input.Name);
            section.Order = input.Order;
            FaqSection faqSection = await FaqSectionRepository.InsertAsync(section);
            return ObjectMapper.Map<FaqSection, FaqSectionDto>(section);
        }

        [Authorize(AbpCmsKitProAdminPermissions.Faqs.Update)]
        public virtual async Task<FaqSectionDto> UpdateAsync(Guid id, UpdateFaqSectionDto input)
        {
            FaqSection section = await FaqSectionRepository.GetAsync(id);
            await FaqSectionManager.UpdateAsync(section, input.GroupName, input.Name);
            section.Order = input.Order;
            FaqSection faqSection = await FaqSectionRepository.UpdateAsync(section);
            return ObjectMapper.Map<FaqSection, FaqSectionDto>(section);
        }

        [Authorize(AbpCmsKitProAdminPermissions.Faqs.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await FaqSectionRepository.DeleteAsync(id, false);
        }

        public virtual async Task<Dictionary<string, FaqGroupInfoDto>> GetGroupsAsync()
        {
            return await Task.FromResult(ObjectMapper.Map<Dictionary<string, FaqGroupInfo>, Dictionary<string, FaqGroupInfoDto>>(FaqOptions.Groups.ToDictionary()));
        }
    }
}
