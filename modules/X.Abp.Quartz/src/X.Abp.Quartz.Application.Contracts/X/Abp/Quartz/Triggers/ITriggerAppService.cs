// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Threading.Tasks;

using Volo.Abp.Application.Services;

using X.Abp.Quartz.Dtos;
using X.Abp.Quartz.Triggers.Dtos;

namespace X.Abp.Quartz.Triggers;

public interface ITriggerAppService : IApplicationService
{
    Task<IReadOnlyList<JobOrTriggerKeyDto>> GetListAsync(string schedulerName, GroupMatcherDto groupMatcher);

    Task<TriggerDetailDto> GetAsync(string schedulerName, string triggerGroup, string triggerName);

    Task PauseAsync(string schedulerName, string triggerGroup, string triggerName);

    Task BatchPauseAsync(string schedulerName, GroupMatcherDto groupMatcher);

    Task ResumeAsync(string schedulerName, string triggerGroup, string triggerName);

    Task BatchResumeAsync(string schedulerName, GroupMatcherDto groupMatcher);
}
