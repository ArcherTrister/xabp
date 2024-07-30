// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

using X.Abp.Quartz.Dtos;
using X.Abp.Quartz.Jobs.Dtos;

namespace X.Abp.Quartz.Jobs;

public interface IJobAppService : IApplicationService
{
    Task<PagedResultDto<JobOrTriggerKeyDto>> GetListAsync([Required] string schedulerName, GetJobListInput input);

    Task<JobDetailDto> GetAsync([Required] string schedulerName, string jobGroup, string jobName);

    Task<PagedResultDto<CurrentlyExecutingJobDto>> CurrentlyExecutingsAsync([Required] string schedulerName, GetJobListInput input);

    Task PauseAsync([Required] string schedulerName, string jobGroup, string jobName);

    Task BatchPauseAsync([Required] string schedulerName, GroupMatcherDto groupMatcher);

    Task ResumeAsync([Required] string schedulerName, string jobGroup, string jobName);

    Task BatchResumeAsync([Required] string schedulerName, GroupMatcherDto groupMatcher);

    Task TriggerAsync([Required] string schedulerName, string jobGroup, string jobName);

    Task DeleteAsync([Required] string schedulerName, string jobGroup, string jobName);

    Task InterruptAsync([Required] string schedulerName, string jobGroup, string jobName);

    Task CreateAsync([Required] string schedulerName, AddJobInput input);
}
