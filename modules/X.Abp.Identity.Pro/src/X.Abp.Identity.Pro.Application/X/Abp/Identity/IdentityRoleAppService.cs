// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Data;
using Volo.Abp.Identity;
using Volo.Abp.ObjectExtending;

using X.Abp.Identity.Permissions;

namespace X.Abp.Identity;

[Authorize(AbpIdentityProPermissions.Roles.Default)]
public class IdentityRoleAppService : IdentityAppServiceBase, IIdentityRoleAppService
{
    protected IIdentityUserRepository UserRepository { get; }

    protected IdentityUserManager UserManager { get; }

    protected IdentityRoleManager RoleManager { get; }

    protected IIdentityRoleRepository RoleRepository { get; }

    protected IIdentityClaimTypeRepository IdentityClaimTypeRepository { get; }

    public IdentityRoleAppService(
        IIdentityUserRepository userRepository,
        IdentityUserManager userManager,
        IdentityRoleManager roleManager,
        IIdentityRoleRepository roleRepository,
        IIdentityClaimTypeRepository identityClaimTypeRepository)
    {
        UserRepository = userRepository;
        UserManager = userManager;
        RoleManager = roleManager;
        RoleRepository = roleRepository;
        IdentityClaimTypeRepository = identityClaimTypeRepository;
    }

    public virtual async Task<IdentityRoleDto> GetAsync(Guid id)
    {
        IdentityRoleDto identityRoleDto = ObjectMapper.Map<IdentityRole, IdentityRoleDto>(await RoleManager.GetByIdAsync(id));

        identityRoleDto.UserCount = await UserRepository.GetCountAsync(roleId: id);

        return identityRoleDto;
    }

    public virtual async Task<ListResultDto<IdentityRoleDto>> GetAllListAsync()
    {
        List<IdentityRoleWithUserCount> source = await RoleRepository.GetListWithUserCountAsync(null, false);
        ListResultDto<IdentityRoleDto> allList = new ListResultDto<IdentityRoleDto>(ObjectMapper.Map<List<IdentityRole>, List<IdentityRoleDto>>(source.Select(x => x.Role).ToList()));
        foreach (IdentityRoleDto identityRoleDto in allList.Items)
        {
            identityRoleDto.UserCount = source.First(x => x.Role.Id == identityRoleDto.Id).UserCount;
        }

        return allList;
    }

    public virtual async Task<PagedResultDto<IdentityRoleDto>> GetListAsync(GetIdentityRoleListInput input)
    {
        List<IdentityRoleWithUserCount> list = await RoleRepository.GetListWithUserCountAsync(input.Sorting, input.MaxResultCount, input.SkipCount, input.Filter, false);
        PagedResultDto<IdentityRoleDto> pagedResultDto = new PagedResultDto<IdentityRoleDto>(await RoleRepository.GetCountAsync(input.Filter), ObjectMapper.Map<List<IdentityRole>, List<IdentityRoleDto>>(list.Select(x => x.Role).ToList()));
        foreach (IdentityRoleDto identityRoleDto in pagedResultDto.Items)
        {
            identityRoleDto.UserCount = list.First(x => x.Role.Id == identityRoleDto.Id).UserCount;
        }

        return pagedResultDto;
    }

    [Authorize(AbpIdentityProPermissions.Roles.Update)]
    public virtual async Task UpdateClaimsAsync(Guid id, List<IdentityRoleClaimUpdateDto> input)
    {
        var role = await RoleRepository.GetAsync(id);

        foreach (var claim in input)
        {
            var existing = role.FindClaim(new Claim(claim.ClaimType, claim.ClaimValue));
            if (existing == null)
            {
                role.AddClaim(GuidGenerator, new Claim(claim.ClaimType, claim.ClaimValue));
            }
        }

        foreach (var claim in role.Claims.ToList())
        {
            if (!input.Any(c => claim.ClaimType == c.ClaimType && claim.ClaimValue == c.ClaimValue))
            {
                role.RemoveClaim(new Claim(claim.ClaimType, claim.ClaimValue));
            }
        }

        if (role.Claims.Count != 0)
        {
            var claimTypes = role.Claims.Select(x => x.ClaimType);
            foreach (IdentityClaimType identityClaimType in (await IdentityClaimTypeRepository.GetListByNamesAsync(claimTypes)).Where(x => x.ValueType == IdentityClaimValueType.String))
            {
                IdentityRoleClaim identityRoleClaim = role.Claims.FirstOrDefault(x => x.ClaimType == identityClaimType.Name);
                if (identityRoleClaim != null)
                {
                    if (identityClaimType.Required && identityRoleClaim.ClaimValue.IsNullOrWhiteSpace())
                    {
                        throw new UserFriendlyException(L["ClaimValueCanNotBeBlank"]);
                    }

                    if (!identityClaimType.Regex.IsNullOrWhiteSpace() && !Regex.IsMatch(identityRoleClaim.ClaimValue, identityClaimType.Regex, RegexOptions.None, TimeSpan.FromSeconds(1.0)))
                    {
                        throw new UserFriendlyException(L["ClaimValueIsInvalid", identityClaimType.Name]);
                    }
                }
            }
        }

        await RoleRepository.UpdateAsync(role);
    }

    [Authorize(AbpIdentityProPermissions.Roles.Default)]
    public virtual async Task<List<ClaimTypeDto>> GetAllClaimTypesAsync()
    {
        var claimTypes = await IdentityClaimTypeRepository.GetListAsync();

        var dtos = ObjectMapper.Map<List<IdentityClaimType>, List<ClaimTypeDto>>(claimTypes);
        foreach (var dto in dtos)
        {
            dto.ValueTypeAsString = dto.ValueType.ToString();
        }

        return dtos;
    }

    [Authorize(AbpIdentityProPermissions.Roles.Default)]
    public virtual async Task<List<IdentityRoleClaimDto>> GetClaimsAsync(Guid id)
    {
        var role = await RoleRepository.GetAsync(id);
        return new List<IdentityRoleClaimDto>(
            ObjectMapper.Map<List<IdentityRoleClaim>, List<IdentityRoleClaimDto>>(role.Claims.ToList()));
    }

    [Authorize(AbpIdentityProPermissions.Roles.Update)]
    public virtual async Task MoveAllUsersAsync(Guid id, Guid? targetRoleId)
    {
        await UserManager.UpdateRoleAsync((await RoleManager.GetByIdAsync(id)).Id, targetRoleId);
    }

    [Authorize(AbpIdentityProPermissions.Roles.Create)]
    public virtual async Task<IdentityRoleDto> CreateAsync(IdentityRoleCreateDto input)
    {
        var role = new IdentityRole(
            GuidGenerator.Create(),
            input.Name,
            CurrentTenant.Id)
        {
            IsDefault = input.IsDefault,
            IsPublic = input.IsPublic
        };

        input.MapExtraPropertiesTo(role);

        (await RoleManager.CreateAsync(role)).CheckIdentityErrors();
        await CurrentUnitOfWork.SaveChangesAsync();

        return ObjectMapper.Map<IdentityRole, IdentityRoleDto>(role);
    }

    [Authorize(AbpIdentityProPermissions.Roles.Update)]
    public virtual async Task<IdentityRoleDto> UpdateAsync(Guid id, IdentityRoleUpdateDto input)
    {
        var role = await RoleManager.GetByIdAsync(id);
        role.SetConcurrencyStampIfNotNull(input.ConcurrencyStamp);

        (await RoleManager.SetRoleNameAsync(role, input.Name)).CheckIdentityErrors();

        role.IsDefault = input.IsDefault;
        role.IsPublic = input.IsPublic;

        input.MapExtraPropertiesTo(role);

        (await RoleManager.UpdateAsync(role)).CheckIdentityErrors();
        await CurrentUnitOfWork.SaveChangesAsync();

        return ObjectMapper.Map<IdentityRole, IdentityRoleDto>(role);
    }

    [Authorize(AbpIdentityProPermissions.Roles.Delete)]
    public virtual async Task DeleteAsync(Guid id)
    {
        IdentityRole role = await RoleManager.GetByIdAsync(id);
        await UserManager.UpdateRoleAsync(id, null);
        await RoleManager.DeleteAsync(role);
    }
}
