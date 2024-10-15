// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
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
    public class MongoPageFeedbackRepository : MongoDbRepository<ICmsKitProMongoDbContext, PageFeedback, Guid>, IPageFeedbackRepository
    {
        public MongoPageFeedbackRepository(IMongoDbContextProvider<ICmsKitProMongoDbContext> dbContextProvider)
          : base(dbContextProvider)
        {
        }

        public virtual async Task<List<PageFeedback>> GetListAsync(
          string entityType = null,
          string entityId = null,
          bool? isUseful = null,
          string url = null,
          bool? isHandled = null,
          string sorting = null,
          int skipCount = 0,
          int maxResultCount = 2147483647,
          CancellationToken cancellationToken = default)
        {
            return await (await GetFilteredQueryableAsync(entityType, entityId, isUseful, url, isHandled))
                      .OrderBy(sorting ?? "CreationTime desc")
                      .PageBy(skipCount, maxResultCount)
                      .As<IMongoQueryable<PageFeedback>>()
                      .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<long> GetCountAsync(
          string entityType = null,
          string entityId = null,
          bool? isUseful = null,
          string url = null,
          bool? isHandled = null,
          CancellationToken cancellationToken = default)
        {
            return await (await GetFilteredQueryableAsync(entityType, entityId, isUseful, url, isHandled)).LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual async Task<IMongoQueryable<PageFeedback>> GetFilteredQueryableAsync(
          string entityType = null,
          string entityId = null,
          bool? isUseful = null,
          string url = null,
          bool? isHandled = null)
        {
            return (await GetMongoQueryableAsync(new CancellationToken(), null))
                      .WhereIf(!entityType.IsNullOrWhiteSpace(), pageFeedback => pageFeedback.EntityType == entityType)
                      .WhereIf(!entityId.IsNullOrWhiteSpace(), pageFeedback => pageFeedback.EntityId == entityId)
                      .WhereIf(isUseful.HasValue, pageFeedback => pageFeedback.IsUseful == isUseful)
                      .WhereIf(!url.IsNullOrWhiteSpace(), pageFeedback => pageFeedback.Url == url)
                      .WhereIf(isHandled.HasValue, pageFeedback => pageFeedback.IsHandled == isHandled)
                      .As<IMongoQueryable<PageFeedback>>();
        }
    }
}
