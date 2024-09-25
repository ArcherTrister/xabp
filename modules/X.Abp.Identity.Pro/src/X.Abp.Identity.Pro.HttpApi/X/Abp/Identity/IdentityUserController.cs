// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Content;

namespace X.Abp.Identity;

[RemoteService(Name = AbpIdentityProRemoteServiceConsts.RemoteServiceName)]
[Area(AbpIdentityProRemoteServiceConsts.ModuleName)]
[ControllerName("User")]
[Route("api/identity/users")]
public class IdentityUserController : AbpControllerBase, IIdentityUserAppService
{
    protected IIdentityUserAppService UserAppService { get; }

    public IdentityUserController(IIdentityUserAppService userAppService)
    {
        UserAppService = userAppService;
    }

    [HttpGet]
    [Route("{id}")]
    public virtual Task<IdentityUserDto> GetAsync(Guid id)
    {
        return UserAppService.GetAsync(id);
    }

    [HttpGet]
    public virtual Task<PagedResultDto<IdentityUserDto>> GetListAsync(GetIdentityUsersInput input)
    {
        return UserAppService.GetListAsync(input);
    }

    [HttpPost]
    public virtual Task<IdentityUserDto> CreateAsync(IdentityUserCreateDto input)
    {
        return UserAppService.CreateAsync(input);
    }

    [HttpPut]
    [Route("{id}")]
    public virtual Task<IdentityUserDto> UpdateAsync(Guid id, IdentityUserUpdateDto input)
    {
        return UserAppService.UpdateAsync(id, input);
    }

    [HttpDelete]
    [Route("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return UserAppService.DeleteAsync(id);
    }

    [HttpGet]
    [Route("by-id/{id}")]
    public virtual Task<IdentityUserDto> FindByIdAsync(Guid id)
    {
        return UserAppService.FindByIdAsync(id);
    }

    [HttpGet]
    [Route("by-phone-number/{phoneNumber}")]
    public virtual Task<IdentityUserDto> FindByPhoneNumberAsync(string phoneNumber)
    {
        return UserAppService.FindByPhoneNumberAsync(phoneNumber);
    }

    [HttpGet]
    [Route("{id}/roles")]
    public virtual Task<ListResultDto<IdentityRoleDto>> GetRolesAsync(Guid id)
    {
        return UserAppService.GetRolesAsync(id);
    }

    [HttpGet]
    [Route("assignable-roles")]
    public virtual Task<ListResultDto<IdentityRoleDto>> GetAssignableRolesAsync()
    {
        return UserAppService.GetAssignableRolesAsync();
    }

    [HttpGet]
    [Route("available-organization-units")]
    public virtual Task<
        ListResultDto<OrganizationUnitWithDetailsDto>
    > GetAvailableOrganizationUnitsAsync()
    {
        return UserAppService.GetAvailableOrganizationUnitsAsync();
    }

    [HttpGet]
    [Route("all-claim-types")]
    public virtual Task<List<ClaimTypeDto>> GetAllClaimTypesAsync()
    {
        return UserAppService.GetAllClaimTypesAsync();
    }

    [HttpGet]
    [Route("{id}/claims")]
    public virtual Task<List<IdentityUserClaimDto>> GetClaimsAsync(Guid id)
    {
        return UserAppService.GetClaimsAsync(id);
    }

    [HttpGet]
    [Route("{id}/organization-units")]
    public virtual Task<List<OrganizationUnitDto>> GetOrganizationUnitsAsync(Guid id)
    {
        return UserAppService.GetOrganizationUnitsAsync(id);
    }

    [HttpPut]
    [Route("{id}/roles")]
    public virtual Task UpdateRolesAsync(Guid id, IdentityUserUpdateRolesDto input)
    {
        return UserAppService.UpdateRolesAsync(id, input);
    }

    [HttpPut]
    [Route("{id}/claims")]
    public virtual Task UpdateClaimsAsync(Guid id, List<IdentityUserClaimDto> input)
    {
        return UserAppService.UpdateClaimsAsync(id, input);
    }

    [HttpPut]
    [Route("{id}/lock/{lockoutEnd}")]
    public virtual Task LockAsync(Guid id, DateTime lockoutEnd)
    {
        return UserAppService.LockAsync(id, lockoutEnd);
    }

    [HttpPut]
    [Route("{id}/unlock")]
    public virtual Task UnlockAsync(Guid id)
    {
        return UserAppService.UnlockAsync(id);
    }

    [HttpGet]
    [Route("by-username/{username}")]
    public virtual Task<IdentityUserDto> FindByUsernameAsync(string username)
    {
        return UserAppService.FindByUsernameAsync(username);
    }

    [HttpGet]
    [Route("by-email/{email}")]
    public virtual Task<IdentityUserDto> FindByEmailAsync(string email)
    {
        return UserAppService.FindByEmailAsync(email);
    }

    [HttpGet]
    [Route("{id}/two-factor-enabled")]
    public virtual Task<bool> GetTwoFactorEnabledAsync(Guid id)
    {
        return UserAppService.GetTwoFactorEnabledAsync(id);
    }

    [HttpPut]
    [Route("{id}/two-factor/{enabled}")]
    public virtual Task SetTwoFactorEnabledAsync(Guid id, bool enabled)
    {
        return UserAppService.SetTwoFactorEnabledAsync(id, enabled);
    }

    [HttpPut]
    [Route("{id}/change-password")]
    public virtual Task UpdatePasswordAsync(Guid id, IdentityUserUpdatePasswordInput input)
    {
        return UserAppService.UpdatePasswordAsync(id, input);
    }

    [HttpGet]
    [Route("lookup/roles")]
    public virtual Task<List<IdentityRoleLookupDto>> GetRoleLookupAsync()
    {
        return UserAppService.GetRoleLookupAsync();
    }

    [HttpGet]
    [Route("lookup/organization-units")]
    public virtual Task<List<OrganizationUnitLookupDto>> GetOrganizationUnitLookupAsync()
    {
        return UserAppService.GetOrganizationUnitLookupAsync();
    }

    [HttpGet]
    [Route("external-login-providers")]
    public virtual Task<List<ExternalLoginProviderDto>> GetExternalLoginProvidersAsync()
    {
        return UserAppService.GetExternalLoginProvidersAsync();
    }

    [HttpPost]
    [Route("import-external-user")]
    public virtual Task<IdentityUserDto> ImportExternalUserAsync(ImportExternalUserInput input)
    {
        return UserAppService.ImportExternalUserAsync(input);
    }

    /*
    [HttpPut]
    [Route("{id}/admin-reset-password")]
    public virtual Task AdminResetPasswordAsync(Guid id, AdminResetPasswordInput input)
    {
        return UserAppService.AdminResetPasswordAsync(id, input);
    }
    */

    [HttpGet]
    [Route("export-as-excel")]
    public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(
        GetIdentityUserListAsFileInput input
    )
    {
        return UserAppService.GetListAsExcelFileAsync(input);
    }

    [HttpGet]
    [Route("export-as-csv")]
    public virtual Task<IRemoteStreamContent> GetListAsCsvFileAsync(
        GetIdentityUserListAsFileInput input
    )
    {
        return UserAppService.GetListAsCsvFileAsync(input);
    }

    [HttpGet]
    [Route("download-token")]
    public virtual Task<DownloadTokenResultDto> GetDownloadTokenAsync()
    {
        return UserAppService.GetDownloadTokenAsync();
    }

    [HttpGet]
    [Route("import-users-sample-file")]
    public virtual Task<IRemoteStreamContent> GetImportUsersSampleFileAsync(
        GetImportUsersSampleFileInput input
    )
    {
        return UserAppService.GetImportUsersSampleFileAsync(input);
    }

    [HttpPost]
    [Route("import-users-from-file")]
    public virtual Task<ImportUsersFromFileOutput> ImportUsersFromFileAsync(
        ImportUsersFromFileInputWithStream input
    )
    {
        return UserAppService.ImportUsersFromFileAsync(input);
    }

    [HttpGet]
    [Route("download-import-invalid-users-file")]
    public virtual Task<IRemoteStreamContent> GetImportInvalidUsersFileAsync(
        GetImportInvalidUsersFileInput input
    )
    {
        return UserAppService.GetImportInvalidUsersFileAsync(input);
    }
}
