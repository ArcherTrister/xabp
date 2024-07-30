// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp;
using Volo.Abp.Application.Dtos;

using X.Abp.Quartz.Dtos;
using X.Abp.Quartz.Jobs;
using X.Abp.Quartz.Jobs.Dtos;

namespace X.Abp.Quartz;

[Area(AbpQuartzRemoteServiceConsts.ModuleName)]
[RemoteService(Name = AbpQuartzRemoteServiceConsts.RemoteServiceName)]
[Route("api/quartz/{schedulerName}/jobs")]
public class JobController : QuartzController, IJobAppService
{
    protected IJobAppService JobAppService { get; }

    public JobController(IJobAppService jobAppService)
    {
        JobAppService = jobAppService;
    }

    [HttpGet]
    public async Task<PagedResultDto<JobOrTriggerKeyDto>> GetListAsync([Required] string schedulerName, GetJobListInput input)
    {
        return await JobAppService.GetListAsync(schedulerName, input);
    }

    [HttpGet]
    [Route("{jobGroup}/{jobName}/details")]
    public async Task<JobDetailDto> GetAsync([Required] string schedulerName, string jobGroup, string jobName)
    {
        return await JobAppService.GetAsync(schedulerName, jobGroup, jobName);
    }

    [HttpGet]
    [Route("currently-executings")]
    public async Task<PagedResultDto<CurrentlyExecutingJobDto>> CurrentlyExecutingsAsync([Required] string schedulerName, GetJobListInput input)
    {
        return await JobAppService.CurrentlyExecutingsAsync(schedulerName, input);
    }

    [HttpPost]
    [Route("{jobGroup}/{jobName}/pause")]
    public async Task PauseAsync([Required] string schedulerName, string jobGroup, string jobName)
    {
        await JobAppService.PauseAsync(schedulerName, jobGroup, jobName);
    }

    [HttpPost]
    [Route("pause")]
    public async Task BatchPauseAsync([Required] string schedulerName, GroupMatcherDto groupMatcher)
    {
        await JobAppService.BatchPauseAsync(schedulerName, groupMatcher);
    }

    [HttpPost]
    [Route("{jobGroup}/{jobName}/resume")]
    public async Task ResumeAsync([Required] string schedulerName, string jobGroup, string jobName)
    {
        await JobAppService.ResumeAsync(schedulerName, jobGroup, jobName);
    }

    [HttpPost]
    [Route("resume")]
    public async Task BatchResumeAsync([Required] string schedulerName, GroupMatcherDto groupMatcher)
    {
        await JobAppService.BatchResumeAsync(schedulerName, groupMatcher);
    }

    [HttpPost]
    [Route("{jobGroup}/{jobName}/trigger")]
    public async Task TriggerAsync([Required] string schedulerName, string jobGroup, string jobName)
    {
        await JobAppService.TriggerAsync(schedulerName, jobGroup, jobName);
    }

    [HttpDelete]
    [Route("{jobGroup}/{jobName}")]
    public async Task DeleteAsync([Required] string schedulerName, string jobGroup, string jobName)
    {
        await JobAppService.DeleteAsync(schedulerName, jobGroup, jobName);
    }

    [HttpPost]
    [Route("{jobGroup}/{jobName}/interrupt")]
    public async Task InterruptAsync([Required] string schedulerName, string jobGroup, string jobName)
    {
        await JobAppService.InterruptAsync(schedulerName, jobGroup, jobName);
    }

    [HttpPost]
    public async Task CreateAsync([Required] string schedulerName, AddJobInput input)
    {
        await JobAppService.CreateAsync(schedulerName, input);
    }
}
