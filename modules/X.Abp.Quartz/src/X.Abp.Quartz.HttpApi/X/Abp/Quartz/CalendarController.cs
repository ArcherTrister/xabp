// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp;

using X.Abp.Quartz.Calendars;
using X.Abp.Quartz.Calendars.Dtos;

namespace X.Abp.Quartz;

[Area(AbpQuartzRemoteServiceConsts.ModuleName)]
[RemoteService(Name = AbpQuartzRemoteServiceConsts.RemoteServiceName)]
[Route("api/quartz/{schedulerName}/calendars")]
public class CalendarController : QuartzController, ICalendarAppService
{
    protected ICalendarAppService CalendarAppService { get; }

    public CalendarController(ICalendarAppService calendarAppService)
    {
        CalendarAppService = calendarAppService;
    }

    [HttpGet]
    public async Task<IReadOnlyCollection<string>> GetListAsync(string schedulerName)
    {
        return await CalendarAppService.GetListAsync(schedulerName);
    }

    [HttpGet]
    [Route("{calendarName}")]
    public async Task<CalendarDetailDto> GetAsync(string schedulerName, string calendarName)
    {
        return await CalendarAppService.GetAsync(schedulerName, calendarName);
    }

    [HttpPut]
    [Route("{calendarName}")]
    public async Task CreateAsync(string schedulerName, string calendarName, bool replace, bool updateTriggers)
    {
        await CalendarAppService.CreateAsync(schedulerName, calendarName, replace, updateTriggers);
    }

    [HttpDelete]
    [Route("{calendarName}")]
    public async Task DeleteAsync(string schedulerName, string calendarName)
    {
        await CalendarAppService.DeleteAsync(schedulerName, calendarName);
    }
}
