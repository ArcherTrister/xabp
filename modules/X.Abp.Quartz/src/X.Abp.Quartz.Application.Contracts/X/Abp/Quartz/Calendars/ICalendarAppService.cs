// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Threading.Tasks;

using Volo.Abp.Application.Services;

using X.Abp.Quartz.Calendars.Dtos;

namespace X.Abp.Quartz.Calendars;

public interface ICalendarAppService : IApplicationService
{
    Task<IReadOnlyCollection<string>> GetListAsync(string schedulerName);

    Task<CalendarDetailDto> GetAsync(string schedulerName, string calendarName);

    Task CreateAsync(string schedulerName, string calendarName, bool replace, bool updateTriggers);

    Task DeleteAsync(string schedulerName, string calendarName);
}
