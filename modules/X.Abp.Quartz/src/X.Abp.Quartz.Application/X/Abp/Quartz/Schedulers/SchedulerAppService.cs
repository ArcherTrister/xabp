// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

using Quartz.Impl;

using Volo.Abp.Application.Dtos;

using X.Abp.Quartz.Permissions;
using X.Abp.Quartz.Schedulers.Dtos;

namespace X.Abp.Quartz.Schedulers;

/// <summary>
/// Web API endpoint for scheduler information.
/// </summary>
[Authorize(AbpQuartzPermissions.Schedulers.Default)]
public class SchedulerAppService : QuartzAppServiceBase, ISchedulerAppService
{
    public async Task<PagedResultDto<SchedulerHeaderDto>> GetListAsync(GetSchedulerListInput input)
    {
        var schedulers = await SchedulerRepository.Instance.LookupAll();

        // return schedulers.Select(x => new SchedulerHeaderDto(x)).ToList();
        var query = schedulers.Select(x => new SchedulerHeaderDto(x)).AsQueryable();
        var count = query.LongCount();
        var list = query.MultiOrderBy(string.IsNullOrWhiteSpace(input.Sorting) ? "Name desc" : input.Sorting).Take(input.MaxResultCount).Skip(input.SkipCount).ToList();
        return new PagedResultDto<SchedulerHeaderDto>
        {
            TotalCount = count,
            Items = list
        };
    }

    public async Task<SchedulerDto> GetAsync(string schedulerName)
    {
        var scheduler = await GetSchedulerAsync(schedulerName);
        var metaData = await scheduler.GetMetaData();
        return new SchedulerDto(scheduler, metaData);
    }

    public async Task StartAsync(string schedulerName, int? delayMilliseconds = null)
    {
        var scheduler = await GetSchedulerAsync(schedulerName);
        if (delayMilliseconds == null)
        {
            await scheduler.Start();
        }
        else
        {
            await scheduler.StartDelayed(TimeSpan.FromMilliseconds(delayMilliseconds.Value));
        }
    }

    public async Task StandbyAsync(string schedulerName)
    {
        var scheduler = await GetSchedulerAsync(schedulerName);
        await scheduler.Standby();
    }

    public async Task ShutdownAsync(string schedulerName, bool waitForJobsToComplete = false)
    {
        var scheduler = await GetSchedulerAsync(schedulerName);
        await scheduler.Shutdown(waitForJobsToComplete);
    }

    public async Task ClearAsync(string schedulerName)
    {
        var scheduler = await GetSchedulerAsync(schedulerName);
        await scheduler.Clear();
    }
}
