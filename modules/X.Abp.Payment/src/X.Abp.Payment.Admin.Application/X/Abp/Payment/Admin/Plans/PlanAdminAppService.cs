// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Data;
using Volo.Abp.ObjectExtending;

using X.Abp.Payment.Admin.Permissions;
using X.Abp.Payment.Plans;

namespace X.Abp.Payment.Admin.Plans
{
    [Authorize(AbpPaymentAdminPermissions.Plans.Default)]
    public class PlanAdminAppService : PaymentAdminAppServiceBase, IPlanAdminAppService
    {
        protected IPlanRepository PlanRepository { get; }

        public PlanAdminAppService(IPlanRepository planRepository)
        {
            PlanRepository = planRepository;
        }

        [Authorize(AbpPaymentAdminPermissions.Plans.Delete)]
        public virtual async Task DeleteGatewayPlanAsync(Guid planId, string gateway)
        {
            await PlanRepository.DeleteGatewayPlanAsync(planId, gateway);
        }

        [Authorize(AbpPaymentAdminPermissions.Plans.GatewayPlans.Create)]
        public virtual async Task CreateGatewayPlanAsync(Guid planId, GatewayPlanCreateInput input)
        {
            var gatewayPlan = new GatewayPlan(planId, input.Gateway, input.ExternalId);
            input.MapExtraPropertiesTo(gatewayPlan);
            await PlanRepository.InsertGatewayPlanAsync(gatewayPlan);
        }

        [Authorize(AbpPaymentAdminPermissions.Plans.GatewayPlans.Update)]
        public virtual async Task UpdateGatewayPlanAsync(
          Guid planId,
          string gateway,
          GatewayPlanUpdateInput input)
        {
            GatewayPlan gatewayPlan = await PlanRepository.GetGatewayPlanAsync(planId, gateway);
            gatewayPlan.SetExternalId(input.ExternalId);
            input.MapExtraPropertiesTo(gatewayPlan);
            await PlanRepository.UpdateGatewayPlanAsync(gatewayPlan);
        }

        public virtual async Task<PlanDto> GetAsync(Guid id)
        {
            Plan plan = await PlanRepository.GetAsync(id, true);
            return ObjectMapper.Map<Plan, PlanDto>(plan);
        }

        public virtual async Task<PagedResultDto<PlanDto>> GetListAsync(PlanGetListInput input)
        {
            List<Plan> plans = await PlanRepository.GetPagedAndFilteredListAsync(input.SkipCount, input.MaxResultCount, input.Sorting.IsNullOrEmpty() ? "name desc" : input.Sorting, input.Filter, true);
            return new PagedResultDto<PlanDto>(await PlanRepository.GetFilteredCountAsync(input.Filter), ObjectMapper.Map<List<Plan>, List<PlanDto>>(plans));
        }

        [Authorize(AbpPaymentAdminPermissions.Plans.Create)]
        public virtual async Task<PlanDto> CreateAsync(PlanCreateInput input)
        {
            var plan = new Plan(GuidGenerator.Create(), input.Name);
            input.MapExtraPropertiesTo(plan);
            await PlanRepository.InsertAsync(plan, false);
            return ObjectMapper.Map<Plan, PlanDto>(plan);
        }

        [Authorize(AbpPaymentAdminPermissions.Plans.Update)]
        public virtual async Task<PlanDto> UpdateAsync(Guid id, PlanUpdateInput input)
        {
            var configuredTaskAwaitable = PlanRepository.GetAsync(id, true);
            Plan plan = await configuredTaskAwaitable;
            plan.Name = input.Name;
            plan.SetConcurrencyStampIfNotNull(input.ConcurrencyStamp);
            input.MapExtraPropertiesTo(plan);
            await PlanRepository.UpdateAsync(plan, false);
            return ObjectMapper.Map<Plan, PlanDto>(plan);
        }

        [Authorize(AbpPaymentAdminPermissions.Plans.Delete)]
        public virtual Task DeleteAsync(Guid id)
        {
            return PlanRepository.DeleteAsync(id, false);
        }

        [Authorize(AbpPaymentAdminPermissions.Plans.GatewayPlans.Default)]
        public async Task<PagedResultDto<GatewayPlanDto>> GetGatewayPlansAsync(Guid planId, GatewayPlanGetListInput input)
        {
            List<GatewayPlan> gatewayPlans = await PlanRepository.GetGatewayPlanPagedListAsync(planId, input.SkipCount, input.MaxResultCount, input.Sorting, input.Filter);
            return new PagedResultDto<GatewayPlanDto>(await PlanRepository.GetGatewayPlanCountAsync(planId, input.Filter), ObjectMapper.Map<ICollection<GatewayPlan>, List<GatewayPlanDto>>(gatewayPlans));
        }
    }
}
