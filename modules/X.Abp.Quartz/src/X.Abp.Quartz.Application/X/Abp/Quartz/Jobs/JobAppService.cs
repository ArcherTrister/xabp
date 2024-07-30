// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

using Quartz;
using Quartz.Impl;

using Volo.Abp.Application.Dtos;

using X.Abp.Quartz.Dtos;
using X.Abp.Quartz.Jobs.Dtos;
using X.Abp.Quartz.Permissions;

namespace X.Abp.Quartz.Jobs;

[Authorize(AbpQuartzPermissions.Jobs.Default)]
public class JobAppService : QuartzAppServiceBase, IJobAppService
{
    public async Task<PagedResultDto<JobOrTriggerKeyDto>> GetListAsync([Required] string schedulerName, GetJobListInput input)
    {
        var scheduler = await GetSchedulerAsync(schedulerName);
        var matcher = (input.GroupMatcher ?? new GroupMatcherDto()).GetJobGroupMatcher();
        var jobKeys = await scheduler.GetJobKeys(matcher);

        // return jobKeys.Select(x => new JobOrTriggerKeyDto(x)).ToList();
        var query = jobKeys.Select(x => new JobOrTriggerKeyDto(x)).AsQueryable();
        var count = query.LongCount();
        var list = query.MultiOrderBy(string.IsNullOrWhiteSpace(input.Sorting) ? "Name desc" : input.Sorting).Take(input.MaxResultCount).Skip(input.SkipCount).ToList();
        return new PagedResultDto<JobOrTriggerKeyDto>
        {
            TotalCount = count,
            Items = list
        };
    }

    public async Task<JobDetailDto> GetAsync([Required] string schedulerName, string jobGroup, string jobName)
    {
        var scheduler = await GetSchedulerAsync(schedulerName);
        var jobDetail = await scheduler.GetJobDetail(new JobKey(jobName, jobGroup));
        return new JobDetailDto(jobDetail);
    }

    public async Task<PagedResultDto<CurrentlyExecutingJobDto>> CurrentlyExecutingsAsync([Required] string schedulerName, GetJobListInput input)
    {
        var scheduler = await GetSchedulerAsync(schedulerName);
        var currentlyExecutingJobs = await scheduler.GetCurrentlyExecutingJobs();

        // return currentlyExecutingJobs.Select(x => new CurrentlyExecutingJobDto(x)).ToList();
        var query = currentlyExecutingJobs.Select(x => new CurrentlyExecutingJobDto(x)).AsQueryable();
        var count = query.LongCount();
        var list = query.MultiOrderBy(string.IsNullOrWhiteSpace(input.Sorting) ? "fireTime desc" : input.Sorting).Take(input.MaxResultCount).Skip(input.SkipCount).ToList();
        return new PagedResultDto<CurrentlyExecutingJobDto>
        {
            TotalCount = count,
            Items = list
        };
    }

    [Authorize(AbpQuartzPermissions.Jobs.Update)]
    public async Task PauseAsync([Required] string schedulerName, string jobGroup, string jobName)
    {
        var scheduler = await GetSchedulerAsync(schedulerName);
        await scheduler.PauseJob(new JobKey(jobName, jobGroup));
    }

    [Authorize(AbpQuartzPermissions.Jobs.Update)]
    public async Task BatchPauseAsync([Required] string schedulerName, GroupMatcherDto groupMatcher)
    {
        var scheduler = await GetSchedulerAsync(schedulerName);
        var matcher = (groupMatcher ?? new GroupMatcherDto()).GetJobGroupMatcher();
        await scheduler.PauseJobs(matcher);
    }

    [Authorize(AbpQuartzPermissions.Jobs.Update)]
    public async Task ResumeAsync([Required] string schedulerName, string jobGroup, string jobName)
    {
        var scheduler = await GetSchedulerAsync(schedulerName);
        await scheduler.ResumeJob(new JobKey(jobName, jobGroup));
    }

    [Authorize(AbpQuartzPermissions.Jobs.Update)]
    public async Task BatchResumeAsync([Required] string schedulerName, GroupMatcherDto groupMatcher)
    {
        var scheduler = await GetSchedulerAsync(schedulerName);
        var matcher = (groupMatcher ?? new GroupMatcherDto()).GetJobGroupMatcher();
        await scheduler.ResumeJobs(matcher);
    }

    /// <summary>
    /// 触发
    /// </summary>
    /// <param name="schedulerName">Scheduler Name</param>
    /// <param name="jobGroup">Job Group</param>
    /// <param name="jobName">Job Name</param>
    [Authorize(AbpQuartzPermissions.Jobs.Update)]
    public async Task TriggerAsync([Required] string schedulerName, string jobGroup, string jobName)
    {
        var scheduler = await GetSchedulerAsync(schedulerName);
        await scheduler.TriggerJob(new JobKey(jobName, jobGroup));
    }

    [Authorize(AbpQuartzPermissions.Jobs.Delete)]
    public async Task DeleteAsync([Required] string schedulerName, string jobGroup, string jobName)
    {
        var scheduler = await GetSchedulerAsync(schedulerName);
        await scheduler.DeleteJob(new JobKey(jobName, jobGroup));
    }

    /// <summary>
    /// 中断Job
    /// </summary>
    /// <param name="schedulerName">Scheduler Name</param>
    /// <param name="jobGroup">Job Group</param>
    /// <param name="jobName">Job Name</param>
    [Authorize(AbpQuartzPermissions.Jobs.Update)]
    public async Task InterruptAsync([Required] string schedulerName, string jobGroup, string jobName)
    {
        var scheduler = await GetSchedulerAsync(schedulerName);
        await scheduler.Interrupt(new JobKey(jobName, jobGroup));
    }

    [Authorize(AbpQuartzPermissions.Jobs.Create)]
    public async Task CreateAsync([Required] string schedulerName, AddJobInput input)
    {
        var scheduler = await GetSchedulerAsync(schedulerName);
        var jobDetail = new JobDetailImpl(input.JobName, input.JobGroup, Type.GetType(input.JobType), input.Durable, input.RequestsRecovery);
        await scheduler.AddJob(jobDetail, input.Replace);
    }
}
