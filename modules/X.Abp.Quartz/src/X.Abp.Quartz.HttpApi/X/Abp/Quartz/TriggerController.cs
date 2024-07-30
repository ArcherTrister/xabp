// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp;

using X.Abp.Quartz.Dtos;
using X.Abp.Quartz.Triggers;
using X.Abp.Quartz.Triggers.Dtos;

namespace X.Abp.Quartz;

[Area(AbpQuartzRemoteServiceConsts.ModuleName)]
[RemoteService(Name = AbpQuartzRemoteServiceConsts.RemoteServiceName)]
[Route("api/quartz/{schedulerName}/triggers")]
public class TriggerController : QuartzController, ITriggerAppService
{
    protected ITriggerAppService TriggerAppService { get; }

    public TriggerController(ITriggerAppService triggerAppService)
    {
        TriggerAppService = triggerAppService;
    }

    [HttpGet]
    public virtual async Task<IReadOnlyList<JobOrTriggerKeyDto>> GetListAsync(string schedulerName, GroupMatcherDto groupMatcher)
    {
        return await TriggerAppService.GetListAsync(schedulerName, groupMatcher);
    }

    [HttpGet]
    [Route("{triggerGroup}/{triggerName}/details")]
    public virtual async Task<TriggerDetailDto> GetAsync(string schedulerName, string triggerGroup, string triggerName)
    {
        return await TriggerAppService.GetAsync(schedulerName, triggerGroup, triggerName);
    }

    [HttpPost]
    [Route("{triggerGroup}/{triggerName}/pause")]
    public virtual async Task PauseAsync(string schedulerName, string triggerGroup, string triggerName)
    {
        await TriggerAppService.PauseAsync(schedulerName, triggerGroup, triggerName);
    }

    [HttpPost]
    [Route("pause")]
    public virtual async Task BatchPauseAsync(string schedulerName, GroupMatcherDto groupMatcher)
    {
        await TriggerAppService.BatchPauseAsync(schedulerName, groupMatcher);
    }

    [HttpPost]
    [Route("{triggerGroup}/{triggerName}/resume")]
    public virtual async Task ResumeAsync(string schedulerName, string triggerGroup, string triggerName)
    {
        await TriggerAppService.ResumeAsync(schedulerName, triggerGroup, triggerName);
    }

    [HttpPost]
    [Route("resume")]
    public virtual async Task BatchResumeAsync(string schedulerName, GroupMatcherDto groupMatcher)
    {
        await TriggerAppService.BatchResumeAsync(schedulerName, groupMatcher);
    }
}
