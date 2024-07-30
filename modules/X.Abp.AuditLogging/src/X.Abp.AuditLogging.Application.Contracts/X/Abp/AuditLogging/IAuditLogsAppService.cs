// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

using X.Abp.AuditLogging.Dtos;

namespace X.Abp.AuditLogging;

public interface IAuditLogsAppService : IApplicationService
{
    Task<PagedResultDto<AuditLogDto>> GetListAsync(GetAuditLogListDto input);

    Task<AuditLogDto> GetAsync(Guid id);

    Task<GetErrorRateOutput> GetErrorRateAsync(GetErrorRateFilter filter);

    Task<GetAverageExecutionDurationPerDayOutput> GetAverageExecutionDurationPerDayAsync(GetAverageExecutionDurationPerDayInput filter);

    Task<PagedResultDto<EntityChangeDto>> GetEntityChangesAsync(GetEntityChangesDto input);

    Task<List<EntityChangeWithUsernameDto>> GetEntityChangesWithUsernameAsync(EntityChangeFilter input);

    Task<EntityChangeWithUsernameDto> GetEntityChangeWithUsernameAsync(Guid entityChangeId);

    Task<EntityChangeDto> GetEntityChangeAsync(Guid entityChangeId);
}
