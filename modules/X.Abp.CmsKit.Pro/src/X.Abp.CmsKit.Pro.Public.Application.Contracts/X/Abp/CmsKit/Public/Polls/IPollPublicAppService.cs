// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Volo.Abp.Application.Services;

namespace X.Abp.CmsKit.Public.Polls;

public interface IPollPublicAppService : IApplicationService
{
    Task<bool> IsWidgetNameAvailableAsync(string widgetName);

    // Task<PollWithDetailsDto> FindByWidgetAsync(string widgetName);
    Task<PollWithDetailsDto> FindByAvailableWidgetAsync(string widgetName);

    Task<PollWithDetailsDto> FindByCodeAsync(string code);

    Task<GetResultDto> GetResultAsync(Guid id);

    Task SubmitVoteAsync(Guid id, SubmitPollInput input);
}
