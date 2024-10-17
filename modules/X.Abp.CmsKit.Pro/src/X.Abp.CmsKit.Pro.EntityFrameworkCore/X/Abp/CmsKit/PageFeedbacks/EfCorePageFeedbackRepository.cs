// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

using X.Abp.CmsKit.EntityFrameworkCore;

namespace X.Abp.CmsKit.PageFeedbacks
{
    public class EfCorePageFeedbackRepository : EfCoreRepository<ICmsKitProDbContext, PageFeedback, Guid>, IPageFeedbackRepository
    {
        public EfCorePageFeedbackRepository(IDbContextProvider<ICmsKitProDbContext> dbContextProvider)
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
          int maxResultCount = int.MaxValue,
          CancellationToken cancellationToken = default)
        {
            return await (await GetFilteredQueryableAsync(entityType, entityId, isUseful, url, isHandled))
                .OrderBy(sorting ?? "CreationTime desc")
                .PageBy(skipCount, maxResultCount)
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

        protected virtual async Task<IQueryable<PageFeedback>> GetFilteredQueryableAsync(
          string entityType = null,
          string entityId = null,
          bool? isUseful = null,
          string url = null,
          bool? isHandled = null)
        {
            return (await GetQueryableAsync()).WhereIf(!entityType.IsNullOrWhiteSpace(), pageFeedback => pageFeedback.EntityType == entityType)
                .WhereIf(!entityId.IsNullOrWhiteSpace(), pageFeedback => pageFeedback.EntityId == entityId)
                .WhereIf(isUseful.HasValue, pageFeedback => pageFeedback.IsUseful == isUseful)
                .WhereIf(!url.IsNullOrWhiteSpace(), pageFeedback => pageFeedback.Url == url)
                .WhereIf(isHandled.HasValue, pageFeedback => pageFeedback.IsHandled == isHandled);
        }
    }
}
