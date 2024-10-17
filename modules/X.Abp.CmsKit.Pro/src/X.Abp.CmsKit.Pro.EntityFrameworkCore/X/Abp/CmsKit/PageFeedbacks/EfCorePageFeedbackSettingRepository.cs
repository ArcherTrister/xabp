// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

using X.Abp.CmsKit.EntityFrameworkCore;

namespace X.Abp.CmsKit.PageFeedbacks
{
    public class EfCorePageFeedbackSettingRepository : EfCoreRepository<ICmsKitProDbContext, PageFeedbackSetting, Guid>, IPageFeedbackSettingRepository
    {
        public EfCorePageFeedbackSettingRepository(
        IDbContextProvider<ICmsKitProDbContext> dbContextProvider)
          : base(dbContextProvider)
        {
        }

        public virtual async Task<List<PageFeedbackSetting>> GetListByEntityTypesAsync(
          List<string> entityTypes,
          CancellationToken cancellationToken = default)
        {
            return await (await GetQueryableAsync())
                .Where(pageFeedbackSetting => entityTypes.Contains(pageFeedbackSetting.EntityType))
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<PageFeedbackSetting> FindByEntityTypeAsync(
          string entityType,
          CancellationToken cancellationToken = default)
        {
            return await (await GetQueryableAsync()).Where(pageFeedbackSetting => pageFeedbackSetting.EntityType == entityType)
                .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task DeleteOldSettingsAsync(
          List<string> existingEntityTypes,
          CancellationToken cancellationToken = default)
        {
            await (await GetQueryableAsync())
                .Where(pageFeedbackSetting => !existingEntityTypes.Contains(pageFeedbackSetting.EntityType))
                .ExecuteDeleteAsync(GetCancellationToken(cancellationToken));
        }
    }
}
