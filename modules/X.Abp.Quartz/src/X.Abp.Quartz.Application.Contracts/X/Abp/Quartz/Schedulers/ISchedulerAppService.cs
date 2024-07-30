// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

using X.Abp.Quartz.Schedulers.Dtos;

namespace X.Abp.Quartz.Schedulers;

/// <summary>
/// Web API endpoint for scheduler information.
/// </summary>
public interface ISchedulerAppService : IApplicationService
{
    Task<PagedResultDto<SchedulerHeaderDto>> GetListAsync(GetSchedulerListInput input);

    Task<SchedulerDto> GetAsync(string schedulerName);

    Task StartAsync(string schedulerName, int? delayMilliseconds);

    Task StandbyAsync(string schedulerName);

    Task ShutdownAsync(string schedulerName, bool waitForJobsToComplete);

    Task ClearAsync(string schedulerName);
}
