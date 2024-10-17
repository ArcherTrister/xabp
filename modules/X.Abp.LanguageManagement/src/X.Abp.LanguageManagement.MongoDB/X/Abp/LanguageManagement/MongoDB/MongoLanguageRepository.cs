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

namespace X.Abp.LanguageManagement.MongoDB
{
    public class MongoLanguageRepository : MongoDbRepository<ILanguageManagementMongoDbContext, Language, Guid>, ILanguageRepository
    {
        public MongoLanguageRepository(
          IMongoDbContextProvider<ILanguageManagementMongoDbContext> dbContextProvider)
          : base(dbContextProvider)
        {
        }

        public virtual async Task<List<Language>> GetListByIsEnabledAsync(
          bool isEnabled,
          CancellationToken cancellationToken = default)
        {
            return await (await GetMongoQueryableAsync(cancellationToken, null)).Where(language => language.IsEnabled == isEnabled).ToListAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<List<Language>> GetListAsync(
          string sorting = null,
          int maxResultCount = 2147483647,
          int skipCount = 0,
          string filter = null,
          CancellationToken cancellationToken = default)
        {
            return await (await GetMongoQueryableAsync(cancellationToken, null))
                .WhereIf(filter != null, language => language.DisplayName.Contains(filter) || language.CultureName.Contains(filter))
                .OrderBy(AbpStringExtensions.IsNullOrWhiteSpace(sorting) ? nameof(Language.DisplayName) : sorting)
                .As<IMongoQueryable<Language>>()
                .PageBy<Language, IMongoQueryable<Language>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<long> GetCountAsync(
          string filter,
          CancellationToken cancellationToken = default)
        {
            return await (await GetMongoQueryableAsync(cancellationToken, null))
                .WhereIf(filter != null, language => language.DisplayName.Contains(filter) || language.CultureName.Contains(filter))
                .As<IMongoQueryable<Language>>()
                .LongCountAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<bool> AnyAsync(
          string cultureName,
          CancellationToken cancellationToken = default)
        {
            return await (await GetMongoQueryableAsync(cancellationToken, null)).AnyAsync(language => language.CultureName == cultureName, GetCancellationToken(cancellationToken));
        }
    }
}
