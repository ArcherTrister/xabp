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

using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;

using X.Abp.CmsKit.Pro.MongoDB;

namespace X.Abp.CmsKit.Newsletters
{
    public class MongoNewsletterRecordRepository : MongoDbRepository<ICmsKitProMongoDbContext, NewsletterRecord, Guid>, INewsletterRecordRepository
    {
        public MongoNewsletterRecordRepository(IMongoDbContextProvider<ICmsKitProMongoDbContext> dbContextProvider)
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
            CancellationToken token = GetCancellationToken(cancellationToken);
            return await (await GetMongoQueryableAsync(token, null))
                .WhereIf(!preference.IsNullOrWhiteSpace(), newsletterRecord => newsletterRecord.Preferences.Any(newsletterPreference => newsletterPreference.Preference == preference))
                .WhereIf(!source.IsNullOrWhiteSpace(), newsletterRecord => newsletterRecord.Preferences.Any(newsletterPreference => newsletterPreference.Source.Contains(source)))
                .WhereIf(!emailAddress.IsNullOrWhiteSpace(), newsletterRecord => newsletterRecord.EmailAddress.Equals(emailAddress))
                .PageBy(skipCount, maxResultCount)
                .As<IMongoQueryable<NewsletterRecord>>()
                .Select(x => new NewsletterSummaryQueryResultItem
                {
                    Preferences = x.Preferences.Select(p => p.Preference).ToList(),
                    CreationTime = x.CreationTime,
                    EmailAddress = x.EmailAddress,
                    Id = x.Id,
                })
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<NewsletterRecord> FindByEmailAddressAsync(
          string emailAddress,
          bool includeDetails = true,
          CancellationToken cancellationToken = default)
        {
            Check.NotNullOrWhiteSpace(emailAddress, nameof(emailAddress));
            CancellationToken token = GetCancellationToken(cancellationToken);
            return await (await GetMongoQueryableAsync(token, null)).Where(newsletterRecord => newsletterRecord.EmailAddress == emailAddress).FirstOrDefaultAsync(token);
        }

        public virtual async Task<int> GetCountByFilterAsync(
          string preference = null,
          string source = null,
          string emailAddress = null,
          CancellationToken cancellationToken = default)
        {
            CancellationToken token = GetCancellationToken(cancellationToken);
            return await (await GetMongoQueryableAsync(token, null))
                .WhereIf(!preference.IsNullOrWhiteSpace(), newsletterRecord => newsletterRecord.Preferences.Any(newsletterPreference => newsletterPreference.Preference == preference))
                .WhereIf(!preference.IsNullOrWhiteSpace(), newsletterRecord => newsletterRecord.Preferences.Any(newsletterPreference => newsletterPreference.Source == source))
                .WhereIf(!emailAddress.IsNullOrWhiteSpace(), newsletterRecord => newsletterRecord.EmailAddress.Equals(emailAddress))
                .As<IMongoQueryable<NewsletterRecord>>()
                .CountAsync(token);
        }
    }
}
