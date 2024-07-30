// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Asp.Versioning;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace X.Abp.Identity;

[RemoteService(Name = AbpIdentityProRemoteServiceConsts.RemoteServiceName)]
[Area(AbpIdentityProRemoteServiceConsts.ModuleName)]
[ControllerName("OrganizationUnit")]
[Route("api/identity/organization-units")]
public class OrganizationUnitController : AbpControllerBase, IOrganizationUnitAppService
{
    protected IOrganizationUnitAppService OrganizationUnitAppService { get; }

    public OrganizationUnitController(IOrganizationUnitAppService organizationUnitAppService)
    {
        OrganizationUnitAppService = organizationUnitAppService;
    }

    [HttpPut]
    [Route("{id}/roles")]
    public virtual Task AddRolesAsync(Guid id, OrganizationUnitRoleInput input)
    {
        return OrganizationUnitAppService.AddRolesAsync(id, input);
    }

    [HttpPut]
    [Route("{id}/members")]
    public virtual Task AddMembersAsync(Guid id, OrganizationUnitUserInput input)
    {
        return OrganizationUnitAppService.AddMembersAsync(id, input);
    }

    [HttpPost]
    public virtual Task<OrganizationUnitWithDetailsDto> CreateAsync(OrganizationUnitCreateDto input)
    {
        return OrganizationUnitAppService.CreateAsync(input);
    }

    [HttpDelete]
    [Route("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return OrganizationUnitAppService.DeleteAsync(id);
    }

    [HttpGet]
    [Route("{id}")]
    public virtual Task<OrganizationUnitWithDetailsDto> GetAsync(Guid id)
    {
        return OrganizationUnitAppService.GetAsync(id);
    }

    [HttpGet]
    public virtual Task<PagedResultDto<OrganizationUnitWithDetailsDto>> GetListAsync(GetOrganizationUnitInput input)
    {
        return OrganizationUnitAppService.GetListAsync(input);
    }

    [HttpGet]
    [Route("all")]
    public virtual Task<ListResultDto<OrganizationUnitWithDetailsDto>> GetListAllAsync()
    {
        return OrganizationUnitAppService.GetListAllAsync();
    }

    [HttpGet]
    [Route("{id}/roles")]
    public virtual Task<PagedResultDto<IdentityRoleDto>> GetRolesAsync(Guid id, PagedAndSortedResultRequestDto input)
    {
        return OrganizationUnitAppService.GetRolesAsync(id, input);
    }

    [HttpGet]
    [Route("{id}/members")]
    public virtual Task<PagedResultDto<IdentityUserDto>> GetMembersAsync(Guid id, GetIdentityUsersInput input)
    {
        return OrganizationUnitAppService.GetMembersAsync(id, input);
    }

    [HttpPut]
    [Route("{id}/move")]
    public virtual Task MoveAsync(Guid id, OrganizationUnitMoveInput input)
    {
        return OrganizationUnitAppService.MoveAsync(id, input);
    }

    [HttpGet]
    [Route("available-users")]
    public Task<PagedResultDto<IdentityUserDto>> GetAvailableUsersAsync(GetAvailableUsersInput input)
    {
        return OrganizationUnitAppService.GetAvailableUsersAsync(input);
    }

    [HttpGet]
    [Route("available-roles")]
    public Task<PagedResultDto<IdentityRoleDto>> GetAvailableRolesAsync(GetAvailableRolesInput input)
    {
        return OrganizationUnitAppService.GetAvailableRolesAsync(input);
    }

    [HttpPut]
    [Route("{id}")]
    public virtual Task<OrganizationUnitWithDetailsDto> UpdateAsync(Guid id, OrganizationUnitUpdateDto input)
    {
        return OrganizationUnitAppService.UpdateAsync(id, input);
    }

    [HttpDelete]
    [Route("{id}/members/{memberId}")]
    public virtual Task RemoveMemberAsync(Guid id, Guid memberId)
    {
        return OrganizationUnitAppService.RemoveMemberAsync(id, memberId);
    }

    [HttpDelete]
    [Route("{id}/roles/{roleId}")]
    public virtual Task RemoveRoleAsync(Guid id, Guid roleId)
    {
        return OrganizationUnitAppService.RemoveRoleAsync(id, roleId);
    }
}
