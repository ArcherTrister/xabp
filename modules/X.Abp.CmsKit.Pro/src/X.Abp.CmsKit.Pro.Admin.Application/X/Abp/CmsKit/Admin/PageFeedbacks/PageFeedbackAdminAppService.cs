// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Features;
using Volo.Abp.GlobalFeatures;

using X.Abp.CmsKit.Admin.Permissions;
using X.Abp.CmsKit.Features;
using X.Abp.CmsKit.GlobalFeatures;
using X.Abp.CmsKit.PageFeedbacks;

namespace X.Abp.CmsKit.Admin.PageFeedbacks
{
    [Authorize(AbpCmsKitProAdminPermissions.PageFeedbacks.Default)]
    [RequiresFeature(CmsKitProFeatures.PageFeedbackEnable)]
    [RequiresGlobalFeature(PageFeedbackFeature.Name)]
    public class PageFeedbackAdminAppService : CmsKitProAdminAppServiceBase, IPageFeedbackAdminAppService
    {
        protected IPageFeedbackRepository PageFeedbackRepository { get; }

        protected IPageFeedbackEntityTypeDefinitionStore EntityTypeDefinitionStore { get; }

        protected IPageFeedbackSettingRepository PageFeedbackSettingRepository { get; }

        protected PageFeedbackManager PageFeedbackManager { get; }

        public PageFeedbackAdminAppService(
          IPageFeedbackRepository pageFeedbackRepository,
          IPageFeedbackEntityTypeDefinitionStore entityTypeDefinitionStore,
          IPageFeedbackSettingRepository pageFeedbackSettingRepository,
          PageFeedbackManager pageFeedbackManager)
        {
            PageFeedbackRepository = pageFeedbackRepository;
            EntityTypeDefinitionStore = entityTypeDefinitionStore;
            PageFeedbackSettingRepository = pageFeedbackSettingRepository;
            PageFeedbackManager = pageFeedbackManager;
        }

        public virtual async Task<PageFeedbackDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<PageFeedback, PageFeedbackDto>(await PageFeedbackRepository.GetAsync(id));
        }

        public virtual async Task<PagedResultDto<PageFeedbackDto>> GetListAsync(GetPageFeedbackListInput input)
        {
            List<PageFeedback> pageFeedbacks = await PageFeedbackRepository.GetListAsync(input.EntityType, input.EntityId, input.IsUseful, input.Url, input.IsHandled, input.Sorting, input.SkipCount, input.MaxResultCount);
            long count = await PageFeedbackRepository.GetCountAsync(input.EntityType, input.EntityId, input.IsUseful, input.Url, input.IsHandled);
            PagedResultDto<PageFeedbackDto> pagedResultDto = new PagedResultDto<PageFeedbackDto>
            {
                Items = ObjectMapper.Map<List<PageFeedback>, List<PageFeedbackDto>>(pageFeedbacks),
                TotalCount = count
            };
            return pagedResultDto;
        }

        [Authorize(AbpCmsKitProAdminPermissions.PageFeedbacks.Update)]
        public virtual async Task<PageFeedbackDto> UpdateAsync(Guid id, UpdatePageFeedbackDto dto)
        {
            PageFeedback entity = await PageFeedbackRepository.GetAsync(id);
            entity.SetAdminNote(dto.AdminNote);
            entity.IsHandled = dto.IsHandled;
            PageFeedback source = await PageFeedbackRepository.UpdateAsync(entity);
            return ObjectMapper.Map<PageFeedback, PageFeedbackDto>(source);
        }

        [Authorize(AbpCmsKitProAdminPermissions.PageFeedbacks.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await PageFeedbackRepository.DeleteAsync(id, false);
        }

        public virtual async Task<List<string>> GetEntityTypesAsync()
        {
            return (await EntityTypeDefinitionStore.GetPageFeedbackEntityTypeDefinitionsAsync())
            .Select(x => x.EntityType).ToList();
        }

        [Authorize(AbpCmsKitProAdminPermissions.PageFeedbacks.Settings)]
        public virtual async Task<List<PageFeedbackSettingDto>> GetSettingsAsync()
        {
            return ObjectMapper.Map<List<PageFeedbackSetting>, List<PageFeedbackSettingDto>>(await PageFeedbackSettingRepository.GetListAsync());
        }

        [Authorize(AbpCmsKitProAdminPermissions.PageFeedbacks.Settings)]
        public virtual async Task UpdateSettingsAsync(UpdatePageFeedbackSettingsInput input)
        {
            List<PageFeedbackSetting> settingsInDatabase = await PageFeedbackSettingRepository.GetListByEntityTypesAsync(input.Settings.Select(x => x.EntityType).ToList());
            List<PageFeedbackSetting> newSettings = new List<PageFeedbackSetting>();
            foreach (UpdatePageFeedbackSettingDto setting in input.Settings)
            {
                PageFeedbackSetting pageFeedbackSetting = settingsInDatabase.FirstOrDefault(x => x.EntityType == setting.EntityType);

                if (pageFeedbackSetting != null)
                {
                    pageFeedbackSetting.SetEmailAddresses(setting.EmailAddresses);
                }
                else if (setting.EntityType == null)
                {
                    PageFeedbackSetting result = await PageFeedbackSettingRepository.FindByEntityTypeAsync(null);
                    if (result != null)
                    {
                        result.SetEmailAddresses(setting.EmailAddresses);
                        settingsInDatabase.Add(result);
                    }
                    else
                    {
                        newSettings.Add(PageFeedbackManager.CreateDefaultSetting(setting.EmailAddresses));
                    }
                }
                else
                {
                    newSettings.Add(await PageFeedbackManager.CreateSettingForEntityTypeAsync(setting.EntityType, setting.EmailAddresses));
                }
            }

            List<string> existingEntityTypes = await GetEntityTypesAsync();
            existingEntityTypes.Add(null);
            await PageFeedbackSettingRepository.DeleteOldSettingsAsync(existingEntityTypes);

            if (newSettings.Count != 0)
            {
                await PageFeedbackSettingRepository.InsertManyAsync(newSettings);
            }

            if (settingsInDatabase.Count != 0)
            {
                await PageFeedbackSettingRepository.UpdateManyAsync(settingsInDatabase);
            }
        }
    }
}
