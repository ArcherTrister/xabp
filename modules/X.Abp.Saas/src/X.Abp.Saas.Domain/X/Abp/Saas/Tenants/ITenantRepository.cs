// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Volo.Abp.Domain.Repositories;

namespace X.Abp.Saas.Tenants;

public interface ITenantRepository : IBasicRepository<Tenant, Guid>
{
    Task<Tenant> FindByIdAsync(Guid id, bool includeDetails = true, CancellationToken cancellationToken = default);

    Task<Tenant> FindByNameAsync(string normalizedName, bool includeDetails = true, CancellationToken cancellationToken = default);

    Task<List<Tenant>> GetListAsync(string sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, string filter = null, bool includeDetails = false, Guid? editionId = null, DateTime? expirationDateMin = null, DateTime? expirationDateMax = null, TenantActivationState? tenantActivationState = null, CancellationToken cancellationToken = default);

    Task<List<Tenant>> GetListWithSeparateConnectionStringAsync(string connectionName = Volo.Abp.Data.ConnectionStrings.DefaultConnectionStringName, bool includeDetails = false, CancellationToken cancellationToken = default);

    Task<long> GetCountAsync(string filter = null, Guid? editionId = null, DateTime? expirationDateMin = null, DateTime? expirationDateMax = null, TenantActivationState? tenantActivationState = null, CancellationToken cancellationToken = default);

    Task UpdateEditionsAsync(Guid sourceEditionId, Guid? targetEditionId = null, CancellationToken cancellationToken = default);
}
