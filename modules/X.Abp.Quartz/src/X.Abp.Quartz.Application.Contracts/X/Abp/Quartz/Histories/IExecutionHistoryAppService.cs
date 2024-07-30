// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

using X.Abp.Quartz.Histories.Dtos;

namespace X.Abp.Quartz.Histories;

public interface IExecutionHistoryAppService : IApplicationService
{
    Task<PagedResultDto<ExecutionHistoryDto>> GetListAsync(GetExecutionHistoryListInput input);
}
