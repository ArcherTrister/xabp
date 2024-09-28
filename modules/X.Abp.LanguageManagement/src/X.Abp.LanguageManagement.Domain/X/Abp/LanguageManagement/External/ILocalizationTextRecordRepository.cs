// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Volo.Abp.Domain.Repositories;

namespace X.Abp.LanguageManagement.External
{
    public interface ILocalizationTextRecordRepository
        : IBasicRepository<LocalizationTextRecord, Guid>,
            IBasicRepository<LocalizationTextRecord>,
            IReadOnlyBasicRepository<LocalizationTextRecord>,
            IReadOnlyBasicRepository<LocalizationTextRecord, Guid>,
            IRepository
    {
        List<LocalizationTextRecord> GetList(string resourceName, string cultureName);

        Task<List<LocalizationTextRecord>> GetListAsync(
            string resourceName,
            string cultureName,
            CancellationToken cancellationToken = default);

        LocalizationTextRecord Find(string resourceName, string cultureName);

        Task<LocalizationTextRecord> FindAsync(
            string resourceName,
            string cultureName,
            CancellationToken cancellationToken = default);
    }
}
