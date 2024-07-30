// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace X.Abp.Identity;

public interface IIdentityRoleAppService : ICrudAppService<
    IdentityRoleDto,
    Guid,
    GetIdentityRoleListInput,
    IdentityRoleCreateDto,
    IdentityRoleUpdateDto>
{
    Task<ListResultDto<IdentityRoleDto>> GetAllListAsync();

    Task UpdateClaimsAsync(Guid id, List<IdentityRoleClaimDto> input);

    Task<List<ClaimTypeDto>> GetAllClaimTypesAsync();

    Task<List<IdentityRoleClaimDto>> GetClaimsAsync(Guid id);
}
