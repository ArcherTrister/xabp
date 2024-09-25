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

using X.Abp.Saas.Editions;
using X.Abp.Saas.Tenants;

namespace X.Abp.Saas.EntityFrameworkCore;

public class EfCoreEditionRepository :
EfCoreRepository<ISaasDbContext, Edition, Guid>,
IEditionRepository
{
    public EfCoreEditionRepository(
      IDbContextProvider<ISaasDbContext> dbContextProvider)
      : base(dbContextProvider)
    {
    }

    public virtual async Task<List<Edition>> GetListAsync(
      string sorting = null,
      int maxResultCount = int.MaxValue,
      int skipCount = 0,
      string filter = null,
      bool includeDetails = false,
      CancellationToken cancellationToken = default)
    {
        return await GetListInternalAsync(sorting, maxResultCount, skipCount, filter, includeDetails, cancellationToken);
    }

    public virtual async Task<int> GetCountAsync(
      string filter,
      CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
                  .WhereIf(!filter.IsNullOrWhiteSpace(), edition => edition.DisplayName.Contains(filter))
                  .CountAsync(GetCancellationToken(cancellationToken));
    }

    public virtual async Task<bool> CheckNameExistAsync(
      string displayName,
      CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
                  .Where(edition => edition.DisplayName == displayName)
                  .AnyAsync(GetCancellationToken(cancellationToken));
    }

    public virtual async Task<List<EditionWithTenantCount>> GetListWithTenantCountAsync(string sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, string filter = null, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        List<Edition> editions = await GetListInternalAsync(sorting, maxResultCount, skipCount, filter, includeDetails, cancellationToken);
        Guid[] editionIds = editions.Select(x => x.Id).ToArray();

        var list = await (await GetDbContextAsync()).Set<Tenant>()
            .Where(tenant => tenant.EditionId.HasValue && editionIds.Contains(tenant.EditionId.Value))
            .GroupBy(tenant => tenant.EditionId)
            .Select(grouping => new
        {
            EditionId = grouping.Key,
            Count = grouping.Count()
        }).ToListAsync(GetCancellationToken(cancellationToken));

        return editions.Select(edition =>
        {
            EditionWithTenantCount editionWithTenantCount = new EditionWithTenantCount();
            editionWithTenantCount.Edition = edition;
            var data = list.FirstOrDefault(x =>
            {
                Guid? editionId = x.EditionId;
                Guid id = edition.Id;
                if (!editionId.HasValue)
                {
                    return false;
                }

                return !editionId.HasValue || editionId.GetValueOrDefault() == id;
            });
            editionWithTenantCount.TenantCount = data != null ? data.Count : 0L;
            return editionWithTenantCount;
        }).ToList();
    }

    public virtual async Task<Edition> FindByDisplayNameAsync(string displayName, bool includeDetails = true, CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync()).FirstOrDefaultAsync(edition => edition.DisplayName == displayName, GetCancellationToken(cancellationToken));
    }

    protected virtual async Task<List<Edition>> GetListInternalAsync(
  string sorting = null,
  int maxResultCount = int.MaxValue,
  int skipCount = 0,
  string filter = null,
  bool includeDetails = false,
  CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
          .WhereIf(!filter.IsNullOrWhiteSpace(), edition => edition.DisplayName.Contains(filter))
          .OrderBy(sorting.IsNullOrWhiteSpace() ? nameof(Edition.DisplayName) : sorting)
          .PageBy(skipCount, maxResultCount)
          .ToListAsync(GetCancellationToken(cancellationToken));
    }
}
