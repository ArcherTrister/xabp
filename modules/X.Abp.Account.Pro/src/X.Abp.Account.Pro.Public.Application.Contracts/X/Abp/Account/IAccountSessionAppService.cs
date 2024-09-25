// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using X.Abp.Account.Dtos;
using X.Abp.Identity;

namespace X.Abp.Account;

public interface IAccountSessionAppService : IApplicationService
{
    Task<PagedResultDto<IdentitySessionDto>> GetListAsync(GetAccountIdentitySessionListInput input);

    Task<IdentitySessionDto> GetAsync(Guid id);

    Task RevokeAsync(Guid id);
}
