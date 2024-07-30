// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

using X.Abp.CmsKit.EntityFrameworkCore;

namespace X.Abp.CmsKit.Newsletters;

public class EfCoreNewsletterRecordRepository :
EfCoreRepository<CmsKitProDbContext, NewsletterRecord, Guid>,
INewsletterRecordRepository
{
    public EfCoreNewsletterRecordRepository(
      IDbContextProvider<CmsKitProDbContext> dbContextProvider)
      : base(dbContextProvider)
    {
    }

    public async Task<List<NewsletterSummaryQueryResultItem>> GetListAsync(
      string preference = null,
      string source = null,
      int skipCount = 0,
      int maxResultCount = int.MaxValue,
      CancellationToken cancellationToken = default)
    {
#pragma warning disable CA1307 // 为了清晰起见，请指定 StringComparison
        return await (await GetDbSetAsync())
            .WhereIf(!string.IsNullOrWhiteSpace(preference), x => x.Preferences.Any(p => p.Preference == preference))
            .WhereIf(!string.IsNullOrWhiteSpace(source), x => x.Preferences.Any(p => p.Source.Contains(source)))
            .Select(x => new NewsletterSummaryQueryResultItem
            {
                CreationTime = x.CreationTime,
                EmailAddress = x.EmailAddress,
                Id = x.Id,
            })
            .OrderByDescending(x => x.CreationTime)
            .PageBy(skipCount, maxResultCount)
            .ToListAsync(GetCancellationToken(cancellationToken));
#pragma warning restore CA1307 // 为了清晰起见，请指定 StringComparison
    }

    public async Task<NewsletterRecord> FindByEmailAddressAsync(
      string emailAddress,
      bool includeDetails = true,
      CancellationToken cancellationToken = default)
    {
        Check.NotNullOrWhiteSpace(emailAddress, nameof(emailAddress));
        return await (await GetDbSetAsync()).IncludeDetails(includeDetails).Where(newsletterRecord => newsletterRecord.EmailAddress == emailAddress).FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<int> GetCountByFilterAsync(
      string preference = null,
      string source = null,
      CancellationToken cancellationToken = default)
    {
#pragma warning disable CA1307 // 为了清晰起见，请指定 StringComparison
        return await (await GetDbSetAsync()).WhereIf(!preference.IsNullOrWhiteSpace(), newsletterRecord => newsletterRecord.Preferences.Any(newsletterPreference => newsletterPreference.Preference == preference))
            .WhereIf(!source.IsNullOrWhiteSpace(), newsletterRecord => newsletterRecord.Preferences.Any(newsletterPreference => newsletterPreference.Source.Contains(source)))
            .CountAsync(GetCancellationToken(cancellationToken));
#pragma warning restore CA1307 // 为了清晰起见，请指定 StringComparison
    }

    public override async Task<IQueryable<NewsletterRecord>> WithDetailsAsync()
    {
        return (await GetQueryableAsync()).IncludeDetails();
    }
}
