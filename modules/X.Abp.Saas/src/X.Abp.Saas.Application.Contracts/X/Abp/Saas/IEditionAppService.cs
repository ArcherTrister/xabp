// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Volo.Abp.Application.Services;

using X.Abp.Payment.Plans;
using X.Abp.Saas.Dtos;

namespace X.Abp.Saas;

public interface IEditionAppService : ICrudAppService<EditionDto, Guid, GetEditionsInput, EditionCreateDto, EditionUpdateDto>
{
    Task<GetEditionUsageStatisticsResultDto> GetUsageStatisticsAsync();

    Task<List<PlanDto>> GetPlanLookupAsync();

    Task<List<EditionDto>> GetAllListAsync();

    Task MoveAllTenantsAsync(Guid id, Guid? targetEditionId);
}
