// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Volo.Abp.Domain.Repositories;

namespace X.Abp.LanguageManagement;

public interface ILanguageRepository : IBasicRepository<Language, Guid>
{
    Task<List<Language>> GetListByIsEnabledAsync(
        bool isEnabled,
        CancellationToken cancellationToken = default);

    Task<List<Language>> GetListAsync(
        string sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        string filter = null,
        CancellationToken cancellationToken = default);

    Task<long> GetCountAsync(
        string filter,
        CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(string cultureName, CancellationToken cancellationToken = default);
}
