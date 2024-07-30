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

using X.Abp.Saas.Tenants;

namespace X.Abp.Saas.EntityFrameworkCore;

public class EfCoreTenantRepository : EfCoreRepository<ISaasDbContext, Tenant, Guid>, ITenantRepository
{
    public EfCoreTenantRepository(
      IDbContextProvider<ISaasDbContext> dbContextProvider)
      : base(dbContextProvider)
    {
    }

    public virtual async Task<Tenant> FindByIdAsync(
      Guid id,
      bool includeDetails = true,
      CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync()).IncludeDetails(includeDetails)
                  .FirstOrDefaultAsync(t => t.Id == id, GetCancellationToken(cancellationToken));
    }

    public virtual async Task<Tenant> FindByNameAsync(
      string normalizedName,
      bool includeDetails = true,
      CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
                  .IncludeDetails(includeDetails)
                  .FirstOrDefaultAsync(tenant => tenant.NormalizedName == normalizedName, GetCancellationToken(cancellationToken));
    }

    public virtual async Task<List<Tenant>> GetListAsync(
      string sorting = null,
      int maxResultCount = int.MaxValue,
      int skipCount = 0,
      string filter = null,
      bool includeDetails = false,
      Guid? editionId = null,
      DateTime? expirationDateMin = null,
      DateTime? expirationDateMax = null,
      TenantActivationState? tenantActivationState = null,
      CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync()).IncludeDetails(includeDetails)
                  .WhereIf(!filter.IsNullOrWhiteSpace(), tenant => tenant.Name.Contains(filter))
                  .WhereIf(editionId.HasValue, tenant => tenant.EditionId == editionId.Value)
                  .WhereIf(expirationDateMin.HasValue, tenant => tenant.EditionEndDateUtc >= expirationDateMin)
                  .WhereIf(expirationDateMax.HasValue, tenant => tenant.EditionEndDateUtc <= expirationDateMax)
                  .WhereIf(tenantActivationState.HasValue, tenant => tenant.ActivationState == tenantActivationState.Value)
                  .OrderBy(sorting.IsNullOrWhiteSpace() ? nameof(Tenant.Name) : sorting)
                  .PageBy(skipCount, maxResultCount)
                  .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public virtual async Task<List<Tenant>> GetListWithSeparateConnectionStringAsync(
      string connectionName = Volo.Abp.Data.ConnectionStrings.DefaultConnectionStringName,
      bool includeDetails = false,
      CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
                  .IncludeDetails(includeDetails)
                  .Where(tenant => tenant.ConnectionStrings.Any(connectionString => connectionString.Name == connectionName && !string.IsNullOrWhiteSpace(connectionString.Value)))
                  .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public virtual async Task<long> GetCountAsync(
      string filter = null,
      Guid? editionId = null,
      DateTime? expirationDateMin = null,
      DateTime? expirationDateMax = null,
      TenantActivationState? tenantActivationState = null,
      CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
                  .WhereIf(!filter.IsNullOrWhiteSpace(), tenant => tenant.Name.Contains(filter))
                  .WhereIf(editionId.HasValue, tenant => tenant.EditionId == editionId.Value)
                  .WhereIf(expirationDateMin.HasValue, tenant => tenant.EditionEndDateUtc >= expirationDateMin)
                  .WhereIf(expirationDateMax.HasValue, tenant => tenant.EditionEndDateUtc <= expirationDateMax)
                  .WhereIf(tenantActivationState.HasValue, tenant => tenant.ActivationState == tenantActivationState.Value)
                  .CountAsync(GetCancellationToken(cancellationToken));
    }

    public virtual async Task UpdateEditionsAsync(Guid sourceEditionId, Guid? targetEditionId = null, CancellationToken cancellationToken = default)
    {
        await (await GetDbSetAsync()).Where(tenant => tenant.EditionId == sourceEditionId)
            .ExecuteUpdateAsync(setPropertyCalls => setPropertyCalls.SetProperty(tenant => tenant.EditionId, targetEditionId), GetCancellationToken(cancellationToken));
    }

    [Obsolete("Use WithDetailsAsync method.")]
    public override IQueryable<Tenant> WithDetails()
    {
        return GetQueryable().IncludeDetails();
    }

    public override async Task<IQueryable<Tenant>> WithDetailsAsync()
    {
        return (await GetQueryableAsync()).IncludeDetails();
    }
}
