// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.AspNetCore.Mvc;

using X.Abp.Quartz.Localization;

namespace X.Abp.Quartz;

public abstract class QuartzController : AbpControllerBase
{
    protected QuartzController()
    {
        LocalizationResource = typeof(QuartzResource);
    }

    // internal static async Task<IScheduler> GetScheduler(string schedulerName)
    // {
    //    var scheduler = await SchedulerRepository.Instance.Lookup(schedulerName);
    //    if (scheduler == null)
    //    {
    //        //throw new HttpResponseException(HttpStatusCode.NotFound);
    //        throw new KeyNotFoundException($"Scheduler {schedulerName} not found!");
    //    }
    //    return scheduler;
    // }
}
