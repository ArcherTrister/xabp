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

namespace X.Abp.CmsKit.UrlShorting;

public class EfCoreShortenedUrlRepository :
EfCoreRepository<CmsKitProDbContext, ShortenedUrl, Guid>,
IShortenedUrlRepository
{
    public EfCoreShortenedUrlRepository(
      IDbContextProvider<CmsKitProDbContext> dbContextProvider)
      : base(dbContextProvider)
    {
    }

    public async Task<List<ShortenedUrl>> GetListAsync(
      string filter = null,
      string sorting = null,
      int skipCount = 0,
      int maxResultCount = int.MaxValue,
      CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .WhereIf(!filter.IsNullOrWhiteSpace(), shortenedUrl => shortenedUrl.Source.Contains(filter) || shortenedUrl.Target.Contains(filter))
            .OrderBy(sorting.IsNullOrEmpty() ? "CreationTime desc" : sorting)
            .PageBy(skipCount, maxResultCount)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<long> GetCountAsync(string filter = null, CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync()).WhereIf(!filter.IsNullOrWhiteSpace(), shortenedUrl => shortenedUrl.Source.Contains(filter) || shortenedUrl.Target.Contains(filter)).LongCountAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<ShortenedUrl> FindBySourceUrlAsync(
      string sourceUrl,
      CancellationToken cancellationToken = default)
    {
        return await FindAsync(shortenedUrl => shortenedUrl.Source == sourceUrl, true, cancellationToken);
    }
}
