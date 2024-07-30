// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

using X.Abp.Payment.Plans;

namespace X.Abp.Payment.Admin.Plans
{
    public interface IPlanAdminAppService : ICrudAppService<PlanDto, Guid, PlanGetListInput, PlanCreateInput, PlanUpdateInput>
    {
        Task<PagedResultDto<GatewayPlanDto>> GetGatewayPlansAsync(
          Guid planId,
          GatewayPlanGetListInput input);

        Task DeleteGatewayPlanAsync(Guid planId, string gateway);

        Task CreateGatewayPlanAsync(Guid planId, GatewayPlanCreateInput input);

        Task UpdateGatewayPlanAsync(Guid planId, string gateway, GatewayPlanUpdateInput input);
    }
}
