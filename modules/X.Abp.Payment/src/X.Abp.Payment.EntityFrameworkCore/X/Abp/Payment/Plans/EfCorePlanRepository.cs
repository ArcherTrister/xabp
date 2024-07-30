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

using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

using X.Abp.Payment.EntityFrameworkCore;

namespace X.Abp.Payment.Plans;

public class EfCorePlanRepository : EfCoreRepository<IPaymentDbContext, Plan, Guid>, IPlanRepository
{
    public EfCorePlanRepository(IDbContextProvider<IPaymentDbContext> dbContextProvider)
      : base(dbContextProvider)
    {
    }

    public virtual async Task<GatewayPlan> GetGatewayPlanAsync(Guid planId, string gateway)
    {
        return await (await GetDbContextAsync()).Set<GatewayPlan>()
            .FirstOrDefaultAsync(gatewayPlan => gatewayPlan.PlanId == planId && gatewayPlan.Gateway == gateway) ?? throw new EntityNotFoundException(typeof(GatewayPlan));
    }

    public virtual async Task InsertGatewayPlanAsync(GatewayPlan gatewayPlan)
    {
        (await GetDbContextAsync()).Entry(gatewayPlan).State = EntityState.Added;
    }

    public virtual async Task DeleteGatewayPlanAsync(Guid planId, string gateway)
    {
        var gatewayPlan = await GetGatewayPlanAsync(planId, gateway);
        (await GetDbContextAsync()).Entry(gatewayPlan).State = EntityState.Deleted;
    }

    public virtual async Task UpdateGatewayPlanAsync(GatewayPlan gatewayPlan)
    {
        (await GetDbContextAsync()).Entry(gatewayPlan).State = EntityState.Modified;
    }

    public override async Task<IQueryable<Plan>> WithDetailsAsync()
    {
        return (await base.WithDetailsAsync())
            .Include(p => p.GatewayPlans);
    }

    public virtual async Task<List<GatewayPlan>> GetGatewayPlanPagedListAsync(
      Guid planId,
      int skipCount,
      int maxResultCount,
      string sorting,
      string filter)
    {
        var source = (await GetDbContextAsync()).GatewayPlans
            .Where(gatewayPlan => gatewayPlan.PlanId == planId)
            .WhereIf(!filter.IsNullOrEmpty(), p => p.Gateway.Contains(filter) || p.ExternalId.Contains(filter))
            .Skip(skipCount).Take(maxResultCount);
        if (!sorting.IsNullOrEmpty())
        {
            source = source.OrderBy(sorting);
        }

        var planPagedListAsync = await source.ToListAsync(GetCancellationToken());

        return planPagedListAsync;
    }

    public virtual async Task<int> GetGatewayPlanCountAsync(Guid planId, string filter)
    {
        return await (await GetDbContextAsync()).GatewayPlans
            .Where(gatewayPlan => gatewayPlan.PlanId == planId)
            .WhereIf(!filter.IsNullOrEmpty(), p => p.Gateway.Contains(filter) || p.ExternalId.Contains(filter))
                .CountAsync();
    }

    public virtual async Task<List<Plan>> GetManyAsync(Guid[] ids)
    {
        return await (await GetQueryableAsync()).Where(plan => ids.Contains(plan.Id)).ToListAsync();
    }

    public virtual async Task<List<Plan>> GetPagedAndFilteredListAsync(
      int skipCount,
      int maxResultCount,
      string sorting,
      string filter,
      bool includeDetails = false,
      CancellationToken cancellationToken = default)
    {
        var queryable = includeDetails ? await WithDetailsAsync() : await GetQueryableAsync();
        var source = CreateFilteredQuery(queryable, filter).Skip(skipCount).Take(maxResultCount);
        if (!sorting.IsNullOrEmpty())
        {
            source = source.OrderBy(sorting);
        }

        return await source.ToListAsync(GetCancellationToken(cancellationToken));
    }

    public virtual async Task<int> GetFilteredCountAsync(
      string filter,
      CancellationToken cancellationToken = default)
    {
        var queryable = await GetQueryableAsync();
        return await CreateFilteredQuery(queryable, filter).CountAsync(GetCancellationToken(cancellationToken));
    }

    protected virtual IQueryable<Plan> CreateFilteredQuery(IQueryable<Plan> queryable, string filter)
    {
        return queryable.WhereIf(!filter.IsNullOrEmpty(), p => p.Name.Contains(filter));
    }
}
