// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

using Quartz;

using X.Abp.Quartz.Dtos;
using X.Abp.Quartz.Permissions;
using X.Abp.Quartz.Triggers.Dtos;

namespace X.Abp.Quartz.Triggers;

[Authorize(AbpQuartzPermissions.Triggers.Default)]
public class TriggerAppService : QuartzAppServiceBase, ITriggerAppService
{
  public virtual async Task<IReadOnlyList<JobOrTriggerKeyDto>> GetListAsync(string schedulerName, GroupMatcherDto groupMatcher)
  {
    var scheduler = await GetSchedulerAsync(schedulerName);
    var matcher = (groupMatcher ?? new GroupMatcherDto()).GetTriggerGroupMatcher();
    var triggerKeys = await scheduler.GetTriggerKeys(matcher);

    return triggerKeys.Select(x => new JobOrTriggerKeyDto(x)).ToList();
  }

  public virtual async Task<TriggerDetailDto> GetAsync(string schedulerName, string triggerGroup, string triggerName)
  {
    var scheduler = await GetSchedulerAsync(schedulerName);
    var trigger = await scheduler.GetTrigger(new TriggerKey(triggerName, triggerGroup));
    var calendar = trigger.CalendarName != null
        ? await scheduler.GetCalendar(trigger.CalendarName)
        : null;
    return TriggerDetailDto.Create(trigger, calendar);
  }

  [Authorize(AbpQuartzPermissions.Triggers.Update)]
  public virtual async Task PauseAsync(string schedulerName, string triggerGroup, string triggerName)
  {
    var scheduler = await GetSchedulerAsync(schedulerName);
    await scheduler.PauseTrigger(new TriggerKey(triggerName, triggerGroup));
  }

  [Authorize(AbpQuartzPermissions.Triggers.Update)]
  public virtual async Task BatchPauseAsync(string schedulerName, GroupMatcherDto groupMatcher)
  {
    var scheduler = await GetSchedulerAsync(schedulerName);
    var matcher = (groupMatcher ?? new GroupMatcherDto()).GetTriggerGroupMatcher();
    await scheduler.PauseTriggers(matcher);
  }

  [Authorize(AbpQuartzPermissions.Triggers.Update)]
  public virtual async Task ResumeAsync(string schedulerName, string triggerGroup, string triggerName)
  {
    var scheduler = await GetSchedulerAsync(schedulerName);
    await scheduler.ResumeTrigger(new TriggerKey(triggerName, triggerGroup));
  }

  [Authorize(AbpQuartzPermissions.Triggers.Update)]
  public virtual async Task BatchResumeAsync(string schedulerName, GroupMatcherDto groupMatcher)
  {
    var scheduler = await GetSchedulerAsync(schedulerName);
    var matcher = (groupMatcher ?? new GroupMatcherDto()).GetTriggerGroupMatcher();
    await scheduler.ResumeTriggers(matcher);
  }
}
