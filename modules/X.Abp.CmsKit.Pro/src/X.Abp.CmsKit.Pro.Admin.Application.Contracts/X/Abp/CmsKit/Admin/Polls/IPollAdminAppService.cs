// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace X.Abp.CmsKit.Admin.Polls;

public interface IPollAdminAppService :
ICrudAppService<PollWithDetailsDto, PollDto, Guid, GetPollListInput, CreatePollDto, UpdatePollDto>
{
    Task<ListResultDto<PollWidgetDto>> GetWidgetsAsync();

    Task<GetResultDto> GetResultAsync(Guid id);
}
