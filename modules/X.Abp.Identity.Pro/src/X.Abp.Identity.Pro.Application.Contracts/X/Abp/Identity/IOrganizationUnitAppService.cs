﻿// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace X.Abp.Identity;

public interface IOrganizationUnitAppService : IApplicationService
{
    Task<OrganizationUnitWithDetailsDto> GetAsync(Guid id);

    Task<PagedResultDto<OrganizationUnitWithDetailsDto>> GetListAsync(GetOrganizationUnitInput input);

    Task<ListResultDto<OrganizationUnitWithDetailsDto>> GetListAllAsync();

    Task<PagedResultDto<IdentityUserDto>> GetMembersAsync(Guid id, GetIdentityUsersInput input);

    Task RemoveMemberAsync(Guid id, Guid memberId);

    Task<PagedResultDto<IdentityRoleDto>> GetRolesAsync(Guid id, PagedAndSortedResultRequestDto input);

    Task RemoveRoleAsync(Guid id, Guid roleId);

    Task<OrganizationUnitWithDetailsDto> CreateAsync(OrganizationUnitCreateDto input);

    Task DeleteAsync(Guid id);

    Task<OrganizationUnitWithDetailsDto> UpdateAsync(Guid id, OrganizationUnitUpdateDto input);

    Task AddRolesAsync(Guid id, OrganizationUnitRoleInput input);

    Task AddMembersAsync(Guid id, OrganizationUnitUserInput input);

    Task MoveAsync(Guid id, OrganizationUnitMoveInput input);

    Task<PagedResultDto<IdentityUserDto>> GetAvailableUsersAsync(GetAvailableUsersInput input);

    Task<PagedResultDto<IdentityRoleDto>> GetAvailableRolesAsync(GetAvailableRolesInput input);
}
