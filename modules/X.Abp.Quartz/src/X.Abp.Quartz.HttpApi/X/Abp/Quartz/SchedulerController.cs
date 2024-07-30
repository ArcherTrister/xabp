// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp;
using Volo.Abp.Application.Dtos;

using X.Abp.Quartz.Schedulers;
using X.Abp.Quartz.Schedulers.Dtos;

namespace X.Abp.Quartz;

/// <summary>
/// Web API endpoint for scheduler information.
/// </summary>
[Area(AbpQuartzRemoteServiceConsts.ModuleName)]
[RemoteService(Name = AbpQuartzRemoteServiceConsts.RemoteServiceName)]
[Route("api/quartz/schedulers")]
public class SchedulerController : QuartzController, ISchedulerAppService
{
    protected ISchedulerAppService SchedulerAppService { get; }

    public SchedulerController(ISchedulerAppService schedulerAppService)
    {
        SchedulerAppService = schedulerAppService;
    }

    [HttpGet]
    public virtual async Task<PagedResultDto<SchedulerHeaderDto>> GetListAsync(GetSchedulerListInput input)
    {
        return await SchedulerAppService.GetListAsync(input);
    }

    [HttpGet]
    [Route("{schedulerName}")]
    public virtual async Task<SchedulerDto> GetAsync(string schedulerName)
    {
        return await SchedulerAppService.GetAsync(schedulerName);
    }

    [HttpPost]
    [Route("{schedulerName}/start")]
    public virtual async Task StartAsync(string schedulerName, int? delayMilliseconds = null)
    {
        await SchedulerAppService.StartAsync(schedulerName, delayMilliseconds);
    }

    [HttpPost]
    [Route("{schedulerName}/standby")]
    public virtual async Task StandbyAsync(string schedulerName)
    {
        await SchedulerAppService.StandbyAsync(schedulerName);
    }

    [HttpPost]
    [Route("{schedulerName}/shutdown")]
    public virtual async Task ShutdownAsync(string schedulerName, bool waitForJobsToComplete = false)
    {
        await SchedulerAppService.ShutdownAsync(schedulerName, waitForJobsToComplete);
    }

    [HttpPost]
    [Route("{schedulerName}/clear")]
    public virtual async Task ClearAsync(string schedulerName)
    {
        await SchedulerAppService.ClearAsync(schedulerName);
    }
}
