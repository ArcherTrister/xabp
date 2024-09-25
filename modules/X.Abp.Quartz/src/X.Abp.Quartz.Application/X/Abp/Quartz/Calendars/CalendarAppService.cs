// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

using Quartz;

using X.Abp.Quartz.Calendars.Dtos;
using X.Abp.Quartz.Permissions;

namespace X.Abp.Quartz.Calendars;

[Authorize(AbpQuartzPermissions.Calendars.Default)]
public class CalendarAppService : QuartzAppServiceBase, ICalendarAppService
{
  public virtual async Task<IReadOnlyCollection<string>> GetListAsync(string schedulerName)
  {
    var scheduler = await GetSchedulerAsync(schedulerName);
    var calendarNames = await scheduler.GetCalendarNames();

    return calendarNames;
  }

  public virtual async Task<CalendarDetailDto> GetAsync(string schedulerName, string calendarName)
  {
    var scheduler = await GetSchedulerAsync(schedulerName);
    var calendar = await scheduler.GetCalendar(calendarName);
    return CalendarDetailDto.Create(calendar);
  }

  [Authorize(AbpQuartzPermissions.Calendars.Create)]
  public virtual async Task CreateAsync(string schedulerName, string calendarName, bool replace, bool updateTriggers)
  {
    var scheduler = await GetSchedulerAsync(schedulerName);
    ICalendar calendar = null;
    await scheduler.AddCalendar(calendarName, calendar, replace, updateTriggers);
  }

  [Authorize(AbpQuartzPermissions.Calendars.Delete)]
  public virtual async Task DeleteAsync(string schedulerName, string calendarName)
  {
    var scheduler = await GetSchedulerAsync(schedulerName);
    await scheduler.DeleteCalendar(calendarName);
  }
}
