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
using Volo.Abp.Threading;

using X.Abp.LanguageManagement.External;

namespace X.Abp.LanguageManagement.MongoDB
{
    public class MongoLocalizationTextRecordRepository : MongoDbRepository<ILanguageManagementMongoDbContext, LocalizationTextRecord, Guid>, ILocalizationTextRecordRepository
    {
        public MongoLocalizationTextRecordRepository(
          IMongoDbContextProvider<ILanguageManagementMongoDbContext> dbContextProvider)
          : base(dbContextProvider)
        {
        }

        public List<LocalizationTextRecord> GetList(
          string resourceName,
          string cultureName)
        {
            using (Volo.Abp.Uow.UnitOfWorkManager.DisableObsoleteDbContextCreationWarning.SetScoped(true))
            {
                return GetMongoQueryable().Where(localizationTextRecord => localizationTextRecord.ResourceName == resourceName && localizationTextRecord.CultureName == cultureName).ToList();
            }
        }

        public virtual async Task<List<LocalizationTextRecord>> GetListAsync(
          string resourceName,
          string cultureName,
          CancellationToken cancellationToken = default)
        {
            return await (await GetMongoQueryableAsync(GetCancellationToken(cancellationToken), null))
                .Where(localizationTextRecord => localizationTextRecord.ResourceName == resourceName && localizationTextRecord.CultureName == cultureName)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public LocalizationTextRecord? Find(
          string resourceName,
          string cultureName)
        {
            using (Volo.Abp.Uow.UnitOfWorkManager.DisableObsoleteDbContextCreationWarning.SetScoped(true))
            {
                return GetMongoQueryable().Where(localizationTextRecord => localizationTextRecord.ResourceName == resourceName && localizationTextRecord.CultureName == cultureName)
                .FirstOrDefault();
            }
        }

        public virtual async Task<LocalizationTextRecord?> FindAsync(
          string resourceName,
          string cultureName,
          CancellationToken cancellationToken = default)
        {
            return await (await GetMongoQueryableAsync(GetCancellationToken(cancellationToken), null))
                .Where(localizationTextRecord => localizationTextRecord.ResourceName == resourceName && localizationTextRecord.CultureName == cultureName)
                .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
        }
    }
}
