// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;

using X.Abp.CmsKit.Pro.MongoDB;

namespace X.Abp.CmsKit.PageFeedbacks
{
    public class MongoPageFeedbackSettingRepository : MongoDbRepository<ICmsKitProMongoDbContext, PageFeedbackSetting, Guid>, IPageFeedbackSettingRepository
    {
        public MongoPageFeedbackSettingRepository(IMongoDbContextProvider<ICmsKitProMongoDbContext> dbContextProvider)
          : base(dbContextProvider)
        {
        }

        public virtual async Task<List<PageFeedbackSetting>> GetListByEntityTypesAsync(
          List<string> entityTypes,
          CancellationToken cancellationToken = default)
        {
            return await (await GetMongoQueryableAsync(GetCancellationToken(cancellationToken), null))
                .Where(pageFeedbackSetting => entityTypes.Contains(pageFeedbackSetting.EntityType))
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<PageFeedbackSetting> FindByEntityTypeAsync(
          string entityType,
          CancellationToken cancellationToken = default)
        {
            return await (await GetMongoQueryableAsync(GetCancellationToken(cancellationToken), null))
                .Where(pageFeedbackSetting => pageFeedbackSetting.EntityType == entityType)
                .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task DeleteOldSettingsAsync(
          List<string> existingEntityTypes,
          CancellationToken cancellationToken = default)
        {
            await (await GetCollectionAsync(GetCancellationToken(cancellationToken)))
                .DeleteManyAsync(pageFeedbackSetting => !existingEntityTypes.Contains(pageFeedbackSetting.EntityType), GetCancellationToken(cancellationToken));
        }
    }
}
