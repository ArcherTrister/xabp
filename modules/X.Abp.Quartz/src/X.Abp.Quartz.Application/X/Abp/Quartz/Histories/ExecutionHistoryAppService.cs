// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

using Volo.Abp.Application.Dtos;

using X.Abp.Quartz.Histories.Dtos;
using X.Abp.Quartz.Permissions;
using X.Abp.Quartz.Plugins.ExecutionHistory;

namespace X.Abp.Quartz.Histories;

[Authorize(AbpQuartzPermissions.ExecutionHistory.Default)]
public class ExecutionHistoryAppService : QuartzAppServiceBase, IExecutionHistoryAppService
{
    public async Task<PagedResultDto<ExecutionHistoryDto>> GetListAsync(GetExecutionHistoryListInput input)
    {
        var store = ExecutionHistoryPlugin.Store;
        if (store == null)
        {
            // Logger.LogError("History plug-in is not enabled.");
            throw new Volo.Abp.UserFriendlyException("History plug-in is not enabled.");
        }
        else
        {
            return await store.GetPageJobHistoryEntriesAsync(input.SchedulerName, input.SkipCount, input.MaxResultCount, input.Sorting);
        }
    }
}
