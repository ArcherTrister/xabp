// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Volo.Abp.Domain.Services;

namespace X.Abp.CmsKit.PageFeedbacks
{
    public class PageFeedbackManager : DomainService
    {
        protected IPageFeedbackEntityTypeDefinitionStore PageFeedbackEntityTypeDefinitionStore { get; }

        public PageFeedbackManager(IPageFeedbackEntityTypeDefinitionStore pageFeedbackEntityTypeDefinitionStore)
        {
            PageFeedbackEntityTypeDefinitionStore = pageFeedbackEntityTypeDefinitionStore;
        }

        public virtual async Task<PageFeedback> CreateAsync(
          string entityType,
          string entityId,
          string url,
          bool isUseful,
          string userNote)
        {
            if (!await PageFeedbackEntityTypeDefinitionStore.IsDefinedAsync(entityType))
            {
                throw new EntityCantHavePageFeedbackException(entityType);
            }

            return new PageFeedback(GuidGenerator.Create(), entityType, entityId, url, isUseful, userNote, tenantId: CurrentTenant.Id);
        }

        public virtual async Task<PageFeedbackSetting> CreateSettingForEntityTypeAsync(
          string entityType,
          string emailAddresses)
        {
            if (!await PageFeedbackEntityTypeDefinitionStore.IsDefinedAsync(entityType))
            {
                throw new EntityCantHavePageFeedbackException(entityType);
            }

            return new PageFeedbackSetting(GuidGenerator.Create(), entityType, emailAddresses, CurrentTenant.Id);
        }

        public virtual PageFeedbackSetting CreateDefaultSetting(string emailAddresses)
        {
            return new PageFeedbackSetting(GuidGenerator.Create(), null, emailAddresses, CurrentTenant.Id);
        }
    }
}
