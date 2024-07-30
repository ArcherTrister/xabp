// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Asp.Versioning;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

using X.Abp.FileManagement.Permissions;

namespace X.Abp.FileManagement.Directories;

[RemoteService(Name = AbpFileManagementRemoteServiceConsts.RemoteServiceName)]
[Area(AbpFileManagementRemoteServiceConsts.ModuleName)]
[ControllerName("DirectoryDescriptors")]
[Route("api/file-management/directory-descriptor")]
[Authorize(AbpFileManagementPermissions.DirectoryDescriptor.Default)]
public class DirectoryDescriptorController : AbpControllerBase, IDirectoryDescriptorAppService
{
    protected IDirectoryDescriptorAppService DirectoryDescriptorAppService { get; }

    public DirectoryDescriptorController(IDirectoryDescriptorAppService directoryDescriptorAppService)
    {
        DirectoryDescriptorAppService = directoryDescriptorAppService;
    }

    [HttpGet]
    [Route("{id}")]
    public virtual async Task<DirectoryDescriptorDto> GetAsync(Guid id)
    {
        return await DirectoryDescriptorAppService.GetAsync(id);
    }

    [HttpGet]
    [Route("sub-directories")]
    public virtual async Task<ListResultDto<DirectoryDescriptorInfoDto>> GetListAsync(Guid? parentId)
    {
        return await DirectoryDescriptorAppService.GetListAsync(parentId);
    }

    [HttpPost]
    [Authorize(AbpFileManagementPermissions.DirectoryDescriptor.Create)]
    public virtual async Task<DirectoryDescriptorDto> CreateAsync(CreateDirectoryInput input)
    {
        return await DirectoryDescriptorAppService.CreateAsync(input);
    }

    [HttpPost]
    [Route("{id}")]
    [Authorize(AbpFileManagementPermissions.DirectoryDescriptor.Update)]
    public virtual async Task<DirectoryDescriptorDto> RenameAsync(Guid id, RenameDirectoryInput input)
    {
        return await DirectoryDescriptorAppService.RenameAsync(id, input);
    }

    [HttpGet]
    public virtual async Task<PagedResultDto<DirectoryContentDto>> GetContentAsync(DirectoryContentRequestInput input)
    {
        return await DirectoryDescriptorAppService.GetContentAsync(input);
    }

    [HttpDelete]
    [Route("{id}")]
    [Authorize(AbpFileManagementPermissions.DirectoryDescriptor.Delete)]
    public virtual async Task DeleteAsync(Guid id)
    {
        await DirectoryDescriptorAppService.DeleteAsync(id);
    }

    [HttpPost]
    [Route("move")]
    [Authorize(AbpFileManagementPermissions.DirectoryDescriptor.Update)]
    public virtual async Task<DirectoryDescriptorDto> MoveAsync(MoveDirectoryInput input)
    {
        return await DirectoryDescriptorAppService.MoveAsync(input);
    }
}
