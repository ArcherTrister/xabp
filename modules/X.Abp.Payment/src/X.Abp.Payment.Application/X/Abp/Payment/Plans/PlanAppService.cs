// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace X.Abp.Payment.Plans;

public class PlanAppService :
PaymentAppServiceBase,
IPlanAppService
{
    protected IPlanRepository PlanRepository { get; }

    public PlanAppService(IPlanRepository planRepository)
    {
        PlanRepository = planRepository;
    }

    public async Task<GatewayPlanDto> GetGatewayPlanAsync(
      Guid planId,
      string gateway)
    {
        var source = await PlanRepository.GetGatewayPlanAsync(planId, gateway);
        return ObjectMapper.Map<GatewayPlan, GatewayPlanDto>(source);
    }

    public async Task<List<PlanDto>> GetPlanListAsync()
    {
        var source = await PlanRepository.GetListAsync();
        return ObjectMapper.Map<List<Plan>, List<PlanDto>>(source);
    }

    public async Task<PlanDto> GetAsync(Guid planId)
    {
        var source = await PlanRepository.FindAsync(planId);
        return source != null ? ObjectMapper.Map<Plan, PlanDto>(source) : null;
    }

    public async Task<List<PlanDto>> GetManyAsync(Guid[] planIds)
    {
        var source = await PlanRepository.GetManyAsync(planIds);
        return ObjectMapper.Map<List<Plan>, List<PlanDto>>(source);
    }
}
