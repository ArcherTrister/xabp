// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Threading.Tasks;

using Quartz;
using Quartz.Impl;

using Volo.Abp.Application.Services;

using X.Abp.Quartz.Localization;

namespace X.Abp.Quartz;

public abstract class QuartzAppServiceBase : ApplicationService
{
    protected QuartzAppServiceBase()
    {
        LocalizationResource = typeof(QuartzResource);
        ObjectMapperContext = typeof(AbpQuartzApplicationModule);
    }

    internal static async Task<IScheduler> GetSchedulerAsync(string schedulerName)
    {
        var scheduler = await SchedulerRepository.Instance.Lookup(schedulerName);
        if (scheduler == null)
        {
            // throw new HttpResponseException(HttpStatusCode.NotFound);
            throw new KeyNotFoundException($"Scheduler {schedulerName} not found!");
        }

        return scheduler;
    }
}
