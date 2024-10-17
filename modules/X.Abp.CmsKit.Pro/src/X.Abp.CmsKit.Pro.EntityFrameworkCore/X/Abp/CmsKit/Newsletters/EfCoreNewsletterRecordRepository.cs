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

public class EfCoreNewsletterRecordRepository : EfCoreRepository<ICmsKitProDbContext, NewsletterRecord, Guid>, INewsletterRecordRepository
{
#pragma warning disable CA1307 // 为了清晰起见，请指定 StringComparison
#pragma warning disable CA1309 // 使用序数字符串比较

    public EfCoreNewsletterRecordRepository(
    IDbContextProvider<ICmsKitProDbContext> dbContextProvider)
    : base(dbContextProvider)
    {
    }

    public virtual async Task<List<NewsletterSummaryQueryResultItem>> GetListAsync(
      string preference = null,
      string source = null,
      string emailAddress = null,
      int skipCount = 0,
      int maxResultCount = int.MaxValue,
      CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .WhereIf(!string.IsNullOrWhiteSpace(preference), x => x.Preferences.Any(p => p.Preference == preference))
            .WhereIf(!string.IsNullOrWhiteSpace(source), x => x.Preferences.Any(p => p.Source.Contains(source)))
            .WhereIf(!emailAddress.IsNullOrWhiteSpace(), newsletterRecord => newsletterRecord.EmailAddress.Equals(emailAddress))
            .Select(x => new NewsletterSummaryQueryResultItem
            {
                Preferences = x.Preferences.Select(p => p.Preference).ToList(),
                CreationTime = x.CreationTime,
                EmailAddress = x.EmailAddress,
                Id = x.Id,
            })
            .OrderByDescending(x => x.CreationTime)
            .PageBy(skipCount, maxResultCount)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public virtual async Task<NewsletterRecord> FindByEmailAddressAsync(
      string emailAddress,
      bool includeDetails = true,
      CancellationToken cancellationToken = default)
    {
        Check.NotNullOrWhiteSpace(emailAddress, nameof(emailAddress));
        return await (await GetDbSetAsync()).IncludeDetails(includeDetails).Where(newsletterRecord => newsletterRecord.EmailAddress == emailAddress).FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
    }

    public virtual async Task<int> GetCountByFilterAsync(
      string preference = null,
      string source = null,
      string emailAddress = null,
      CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .WhereIf(!preference.IsNullOrWhiteSpace(), newsletterRecord => newsletterRecord.Preferences.Any(newsletterPreference => newsletterPreference.Preference == preference))
            .WhereIf(!source.IsNullOrWhiteSpace(), newsletterRecord => newsletterRecord.Preferences.Any(newsletterPreference => newsletterPreference.Source.Contains(source)))
            .WhereIf(!emailAddress.IsNullOrWhiteSpace(), newsletterRecord => newsletterRecord.EmailAddress.Equals(emailAddress))
            .CountAsync(GetCancellationToken(cancellationToken));
    }

    public override async Task<IQueryable<NewsletterRecord>> WithDetailsAsync()
    {
        return (await GetQueryableAsync()).IncludeDetails();
    }
}
