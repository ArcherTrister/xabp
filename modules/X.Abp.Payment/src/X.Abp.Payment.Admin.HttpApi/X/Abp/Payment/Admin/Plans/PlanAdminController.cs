// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Volo.Abp;
using Volo.Abp.Application.Dtos;

using X.Abp.Payment.Admin.Permissions;
using X.Abp.Payment.Plans;

namespace X.Abp.Payment.Admin.Plans
{
    [Area(AbpPaymentAdminRemoteServiceConsts.ModuleName)]
    [RemoteService(true, Name = AbpPaymentAdminRemoteServiceConsts.RemoteServiceName)]
    [Route("api/payment-admin/plans")]
    [Authorize(AbpPaymentAdminPermissions.Plans.Default)]
    public class PlanAdminController :
    PaymentAdminController,
    IPlanAdminAppService
    {
        protected IPlanAdminAppService PlanAdminAppService { get; }

        public PlanAdminController(IPlanAdminAppService planAdminAppService)
        {
            PlanAdminAppService = planAdminAppService;
        }

        [Authorize(AbpPaymentAdminPermissions.Plans.Create)]
        [HttpPost]
        public virtual Task<PlanDto> CreateAsync(PlanCreateInput input)
        {
            return PlanAdminAppService.CreateAsync(input);
        }

        [Route("{planId}/external-plans")]
        [Authorize(AbpPaymentAdminPermissions.Plans.Update)]
        [HttpPost]
        public virtual Task CreateGatewayPlanAsync(Guid planId, GatewayPlanCreateInput input)
        {
            return PlanAdminAppService.CreateGatewayPlanAsync(planId, input);
        }

        [Authorize(AbpPaymentAdminPermissions.Plans.Delete)]
        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return PlanAdminAppService.DeleteAsync(id);
        }

        [Route("{planId}/external-plans/{gateway}")]
        [Authorize(AbpPaymentAdminPermissions.Plans.Update)]
        [HttpDelete]
        public virtual Task DeleteGatewayPlanAsync(Guid planId, string gateway)
        {
            return PlanAdminAppService.DeleteGatewayPlanAsync(planId, gateway);
        }

        [Route("{id}")]
        [HttpGet]
        public virtual Task<PlanDto> GetAsync(Guid id)
        {
            return PlanAdminAppService.GetAsync(id);
        }

        [HttpGet]
        public virtual Task<PagedResultDto<PlanDto>> GetListAsync(PlanGetListInput input)
        {
            return PlanAdminAppService.GetListAsync(input);
        }

        [Authorize(AbpPaymentAdminPermissions.Plans.Update)]
        [Route("{id}")]
        [HttpPut]
        public virtual Task<PlanDto> UpdateAsync(Guid id, PlanUpdateInput input)
        {
            return PlanAdminAppService.UpdateAsync(id, input);
        }

        [Authorize(AbpPaymentAdminPermissions.Plans.Update)]
        [Route("{planId}/external-plans/{gateway}")]
        [HttpPut]
        public virtual Task UpdateGatewayPlanAsync(
          Guid planId,
          string gateway,
          GatewayPlanUpdateInput input)
        {
            return PlanAdminAppService.UpdateGatewayPlanAsync(planId, gateway, input);
        }

        [HttpGet("{planId}/external-plans")]
        public Task<PagedResultDto<GatewayPlanDto>> GetGatewayPlansAsync(
          Guid planId,
          GatewayPlanGetListInput input)
        {
            return PlanAdminAppService.GetGatewayPlansAsync(planId, input);
        }
    }
}
