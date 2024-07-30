// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Threading;

using X.Abp.LanguageManagement.External;

namespace X.Abp.LanguageManagement.EntityFrameworkCore
{
    public class EfCoreLocalizationTextRecordRepository
        : EfCoreRepository<ILanguageManagementDbContext, LocalizationTextRecord, Guid>,
            ILocalizationTextRecordRepository
    {
        public EfCoreLocalizationTextRecordRepository(
            IDbContextProvider<ILanguageManagementDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        [Obsolete("Use GetListAsync() method.")]
        public List<LocalizationTextRecord> GetList(string resourceName, string cultureName)
        {
            using (Volo.Abp.Uow.UnitOfWorkManager.DisableObsoleteDbContextCreationWarning.SetScoped(true))
            {
                return DbSet
                .Where(localizationTextRecord =>
                    localizationTextRecord.ResourceName == resourceName
                    && localizationTextRecord.CultureName == cultureName)
                .ToList();
            }
        }

        public async Task<List<LocalizationTextRecord>> GetListAsync(
            string resourceName,
            string cultureName,
            CancellationToken cancellationToken = default
        )
        {
            return await (await GetDbSetAsync())
                .Where(localizationTextRecord =>
                    localizationTextRecord.ResourceName == resourceName
                    && localizationTextRecord.CultureName == cultureName
                )
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        [Obsolete("Use FindAsync() method.")]
        public LocalizationTextRecord Find(string resourceName, string cultureName)
        {
            using (Volo.Abp.Uow.UnitOfWorkManager.DisableObsoleteDbContextCreationWarning.SetScoped(true))
            {
                return DbSet.FirstOrDefault(
                    localizationTextRecord =>
                        localizationTextRecord.ResourceName == resourceName
                        && localizationTextRecord.CultureName == cultureName);
            }
        }

        public async Task<LocalizationTextRecord> FindAsync(
            string resourceName,
            string cultureName,
            CancellationToken cancellationToken = default
        )
        {
            return await (await GetDbSetAsync())
                .Where(localizationTextRecord =>
                    localizationTextRecord.ResourceName == resourceName
                    && localizationTextRecord.CultureName == cultureName
                )
                .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
        }
    }
}
