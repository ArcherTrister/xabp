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

using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace X.Abp.LanguageManagement.EntityFrameworkCore;

public class EfCoreLanguageRepository : EfCoreRepository<ILanguageManagementDbContext, Language, Guid>,
    ILanguageRepository
{
  public EfCoreLanguageRepository(IDbContextProvider<ILanguageManagementDbContext> dbContextProvider)
      : base(dbContextProvider)
  {
  }

  public virtual async Task<List<Language>> GetListByIsEnabledAsync(
      bool isEnabled,
      CancellationToken cancellationToken = default)
  {
    return await (await GetDbSetAsync())
        .Where(l => l.IsEnabled == isEnabled)
        .ToListAsync(GetCancellationToken(cancellationToken));
  }

  public virtual async Task<List<Language>> GetListAsync(
      string sorting = null,
      int maxResultCount = int.MaxValue,
      int skipCount = 0,
      string filter = null,
      CancellationToken cancellationToken = default)
  {
    return await (await GetQueryableAsync())
        .WhereIf(filter != null,
            x => x.DisplayName.Contains(filter) ||
                 x.CultureName.Contains(filter))
        .OrderBy(sorting.IsNullOrWhiteSpace() ? nameof(Language.DisplayName) : sorting)
        .PageBy(skipCount, maxResultCount)
        .ToListAsync(GetCancellationToken(cancellationToken));
  }

  public virtual async Task<long> GetCountAsync(
      string filter,
      CancellationToken cancellationToken = default)
  {
    return await (await GetQueryableAsync())
        .WhereIf(filter != null,
            x => x.DisplayName.Contains(filter) ||
                 x.CultureName.Contains(filter))
        .CountAsync(GetCancellationToken(cancellationToken));
  }

  public virtual async Task<bool> AnyAsync(
    string cultureName,
    CancellationToken cancellationToken = default)
  {
    return await (await GetDbSetAsync()).AnyAsync(language => language.CultureName == cultureName, GetCancellationToken(cancellationToken));
  }
}
