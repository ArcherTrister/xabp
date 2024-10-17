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

namespace X.Abp.CmsKit.UrlShorting
{
    public class MongoShortenedUrlRepository : MongoDbRepository<ICmsKitProMongoDbContext, ShortenedUrl, Guid>, IShortenedUrlRepository
    {
        public MongoShortenedUrlRepository(IMongoDbContextProvider<ICmsKitProMongoDbContext> dbContextProvider)
          : base(dbContextProvider)
        {
        }

        public virtual async Task<List<ShortenedUrl>> GetListAsync(
          string filter = null,
          string sorting = null,
          int skipCount = 0,
          int maxResultCount = int.MaxValue,
          CancellationToken cancellationToken = default)
        {
            CancellationToken token = GetCancellationToken(cancellationToken);
            return await (await GetMongoQueryableAsync(token, null))
                .WhereIf(!filter.IsNullOrWhiteSpace(), shortenedUrl => shortenedUrl.Source.Contains(filter) || shortenedUrl.Target.Contains(filter))
                .OrderBy(sorting.IsNullOrEmpty() ? "CreationTime desc" : sorting)
                .As<IMongoQueryable<ShortenedUrl>>()
                .PageBy<ShortenedUrl, IMongoQueryable<ShortenedUrl>>(skipCount, maxResultCount)
                .ToListAsync(token);
        }

        public virtual async Task<long> GetCountAsync(
          string filter = null,
          CancellationToken cancellationToken = default)
        {
            return await (await GetMongoQueryableAsync(cancellationToken, null))
                .WhereIf(!filter.IsNullOrWhiteSpace(), shortenedUrl => shortenedUrl.Source.Contains(filter) || shortenedUrl.Target.Contains(filter))
                .As<IMongoQueryable<ShortenedUrl>>()
                .LongCountAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<ShortenedUrl> FindBySourceUrlAsync(
          string sourceUrl,
          CancellationToken cancellationToken = default)
        {
            return await FindAsync(shortenedUrl => shortenedUrl.Source == sourceUrl, cancellationToken: cancellationToken);
        }
    }
}
