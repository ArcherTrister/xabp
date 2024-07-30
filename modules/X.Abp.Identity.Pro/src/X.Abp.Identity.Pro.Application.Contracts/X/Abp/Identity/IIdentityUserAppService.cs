// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace X.Abp.Identity;

public interface IIdentityUserAppService : ICrudAppService<IdentityUserDto, Guid, GetIdentityUsersInput, IdentityUserCreateDto, IdentityUserUpdateDto>
{
    Task<ListResultDto<IdentityRoleDto>> GetRolesAsync(Guid id);

    Task<ListResultDto<IdentityRoleDto>> GetAssignableRolesAsync();

    Task<ListResultDto<OrganizationUnitWithDetailsDto>> GetAvailableOrganizationUnitsAsync();

    Task<List<ClaimTypeDto>> GetAllClaimTypesAsync();

    Task<List<IdentityUserClaimDto>> GetClaimsAsync(Guid id);

    Task<List<OrganizationUnitDto>> GetOrganizationUnitsAsync(Guid id);

    Task UpdateRolesAsync(Guid id, IdentityUserUpdateRolesDto input);

    Task UpdateClaimsAsync(Guid id, List<IdentityUserClaimDto> input);

    Task UpdatePasswordAsync(Guid id, IdentityUserUpdatePasswordInput input);

    Task LockAsync(Guid id, DateTime lockoutEnd);

    Task UnlockAsync(Guid id);

    Task<IdentityUserDto> FindByUsernameAsync(string username);

    Task<IdentityUserDto> FindByEmailAsync(string email);

    Task<IdentityUserDto> FindByPhoneNumberAsync(string phoneNumber);

    Task<bool> GetTwoFactorEnabledAsync(Guid id);

    Task SetTwoFactorEnabledAsync(Guid id, bool enabled);

    Task<List<IdentityRoleLookupDto>> GetRoleLookupAsync();

    Task<List<OrganizationUnitLookupDto>> GetOrganizationUnitLookupAsync();

    Task<List<ExternalLoginProviderDto>> GetExternalLoginProvidersAsync();

    Task<IdentityUserDto> ImportExternalUserAsync(ImportExternalUserInput input);

    /*
    /// <summary>
    /// 管理员重置密码
    /// </summary>
    /// <param name="id">用户Id</param>
    /// <param name="input">AdminResetPasswordInput</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task AdminResetPasswordAsync(Guid id, AdminResetPasswordInput input);
    */
}
