﻿// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace X.Abp.Identity;

public interface IIdentitySecurityLogAppService : IApplicationService
{
    Task<PagedResultDto<IdentitySecurityLogDto>> GetListAsync(GetIdentitySecurityLogListInput input);

    Task<IdentitySecurityLogDto> GetAsync(Guid id);

    Task<PagedResultDto<IdentitySecurityLogDto>> GetMyListAsync(GetIdentitySecurityLogListInput input);

    Task<IdentitySecurityLogDto> GetMyAsync(Guid id);
}
