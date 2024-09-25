// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp;
using Volo.Abp.Application.Dtos;

using X.Abp.Quartz.Histories;
using X.Abp.Quartz.Histories.Dtos;

namespace X.Abp.Quartz;

[Area(AbpQuartzRemoteServiceConsts.ModuleName)]
[RemoteService(Name = AbpQuartzRemoteServiceConsts.RemoteServiceName)]
[Route("api/quartz/execution-histories")]
public class ExecutionHistoryController : QuartzController, IExecutionHistoryAppService
{
  protected IExecutionHistoryAppService ExecutionHistoryAppService { get; }

  public ExecutionHistoryController(IExecutionHistoryAppService executionHistoryAppService)
  {
    ExecutionHistoryAppService = executionHistoryAppService;
  }

  [HttpGet]
  public virtual async Task<PagedResultDto<ExecutionHistoryDto>> GetListAsync(GetExecutionHistoryListInput input)
  {
    return await ExecutionHistoryAppService.GetListAsync(input);
  }
}
