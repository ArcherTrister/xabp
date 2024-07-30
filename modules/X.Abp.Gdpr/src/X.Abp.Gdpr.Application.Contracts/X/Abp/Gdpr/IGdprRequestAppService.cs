// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace X.Abp.Gdpr;

public interface IGdprRequestAppService : IApplicationService
{
    Task PrepareUserDataAsync();

    Task<IRemoteStreamContent> GetUserDataAsync(
      Guid requestId,
      string token);

    Task<DownloadTokenResultDto> GetDownloadTokenAsync(Guid requestId);

    Task<bool> IsNewRequestAllowedAsync();

    Task<PagedResultDto<GdprRequestDto>> GetListByUserIdAsync(
      Guid userId);

    Task DeleteUserDataAsync();
}
