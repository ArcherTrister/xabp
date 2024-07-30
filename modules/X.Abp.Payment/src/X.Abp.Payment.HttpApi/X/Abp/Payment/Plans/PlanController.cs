// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp;

namespace X.Abp.Payment.Plans;

[Route("api/payment/plans")]
[Area(AbpPaymentCommonRemoteServiceConsts.ModuleName)]
[RemoteService(true, Name = AbpPaymentCommonRemoteServiceConsts.RemoteServiceName)]
public class PlanController : PaymentCommonController, IPlanAppService
{
    protected IPlanAppService PlanAppService { get; }

    public PlanController(IPlanAppService planAppService)
    {
        PlanAppService = planAppService;
    }

    [HttpGet]
    [Route("{planId}/{gateway}")]
    public Task<GatewayPlanDto> GetGatewayPlanAsync(Guid planId, string gateway)
    {
        return PlanAppService.GetGatewayPlanAsync(planId, gateway);
    }

    [HttpGet]
    public Task<List<PlanDto>> GetPlanListAsync()
    {
        return PlanAppService.GetPlanListAsync();
    }

    [HttpGet]
    [Route("{planId}")]
    public Task<PlanDto> GetAsync(Guid planId)
    {
        return PlanAppService.GetAsync(planId);
    }

    [HttpGet]
    [Route("many")]
    public Task<List<PlanDto>> GetManyAsync(Guid[] planIds)
    {
        return PlanAppService.GetManyAsync(planIds);
    }
}
