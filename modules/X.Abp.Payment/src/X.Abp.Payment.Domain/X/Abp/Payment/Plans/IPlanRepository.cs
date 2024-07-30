// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Volo.Abp.Domain.Repositories;

namespace X.Abp.Payment.Plans;

public interface IPlanRepository :
IBasicRepository<Plan, Guid>
{
    Task<List<Plan>> GetPagedAndFilteredListAsync(
      int skipCount,
      int maxResultCount,
      string sorting,
      string filter,
      bool includeDetails = false,
      CancellationToken cancellationToken = default);

    Task<int> GetFilteredCountAsync(string filter, CancellationToken cancellationToken = default);

    Task<List<Plan>> GetManyAsync(Guid[] ids);

    Task<GatewayPlan> GetGatewayPlanAsync(Guid planId, string gateway);

    Task InsertGatewayPlanAsync(GatewayPlan gatewayPlan);

    Task DeleteGatewayPlanAsync(Guid planId, string gateway);

    Task UpdateGatewayPlanAsync(GatewayPlan gatewayPlan);

    Task<List<GatewayPlan>> GetGatewayPlanPagedListAsync(
      Guid planId,
      int skipCount,
      int maxResultCount,
      string sorting,
      string filter = null);

    Task<int> GetGatewayPlanCountAsync(Guid planId, string filter = null);
}
