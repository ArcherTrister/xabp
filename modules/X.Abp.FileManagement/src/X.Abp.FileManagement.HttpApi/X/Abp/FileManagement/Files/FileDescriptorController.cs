// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Asp.Versioning;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Content;

using X.Abp.FileManagement.Permissions;

namespace X.Abp.FileManagement.Files;

[RemoteService(Name = AbpFileManagementRemoteServiceConsts.RemoteServiceName)]
[Area(AbpFileManagementRemoteServiceConsts.ModuleName)]
[ControllerName("FileDescriptors")]
[Route("api/file-management/file-descriptor")]
[Authorize(AbpFileManagementPermissions.FileDescriptor.Default)]
public class FileDescriptorController : AbpControllerBase, IFileDescriptorAppService
{
    protected IFileDescriptorAppService FileDescriptorAppService { get; }

    public FileDescriptorController(IFileDescriptorAppService fileDescriptorAppService)
    {
        FileDescriptorAppService = fileDescriptorAppService;
    }

    [HttpGet]
    [Route("{id}")]
    [AllowAnonymous]
    public virtual async Task<FileDescriptorDto> GetAsync(Guid id)
    {
        return await FileDescriptorAppService.GetAsync(id);
    }

    [HttpGet]
    public virtual async Task<ListResultDto<FileDescriptorDto>> GetListAsync(Guid? directoryId)
    {
        return await FileDescriptorAppService.GetListAsync(directoryId);
    }

    [HttpPost]
    [Route("upload")]
    [Authorize(AbpFileManagementPermissions.FileDescriptor.Create)]
    public virtual Task<FileDescriptorDto> CreateAsync(Guid? directoryId, CreateFileInputWithStream inputWithStream)
    {
        return FileDescriptorAppService.CreateAsync(directoryId, inputWithStream);
    }

    [HttpPost]
    [Route("move")]
    [Authorize(AbpFileManagementPermissions.FileDescriptor.Update)]
    public virtual async Task<FileDescriptorDto> MoveAsync(MoveFileInput input)
    {
        return await FileDescriptorAppService.MoveAsync(input);
    }

    [HttpPost]
    [Route("pre-upload-info")]
    public virtual async Task<List<FileUploadPreInfoDto>> GetPreInfoAsync(List<FileUploadPreInfoRequest> input)
    {
        return await FileDescriptorAppService.GetPreInfoAsync(input);
    }

    [HttpGet]
    [Route("content")]
    public virtual async Task<byte[]> GetContentAsync(Guid id)
    {
        return await FileDescriptorAppService.GetContentAsync(id);
    }

    [HttpGet]
    [Route("download/{id}/token")]
    public Task<DownloadTokenResultDto> GetDownloadTokenAsync(Guid id)
    {
        return FileDescriptorAppService.GetDownloadTokenAsync(id);
    }

    [HttpPost]
    [Route("{id}")]
    [Authorize(AbpFileManagementPermissions.FileDescriptor.Update)]
    public virtual async Task<FileDescriptorDto> RenameAsync(Guid id, RenameFileInput input)
    {
        return await FileDescriptorAppService.RenameAsync(id, input);
    }

    [HttpDelete]
    [Route("{id}")]
    [Authorize(AbpFileManagementPermissions.FileDescriptor.Delete)]
    public virtual async Task DeleteAsync(Guid id)
    {
        await FileDescriptorAppService.DeleteAsync(id);
    }

    [HttpGet]
    [Route("download/{id}")]
    [AllowAnonymous]
    public virtual async Task<IRemoteStreamContent> DownloadAsync(Guid id, string token)
    {
        var fileDescriptor = await FileDescriptorAppService.GetAsync(id);

        Response.Headers.Add("Content-Disposition", $"attachment;filename=\"{fileDescriptor.Name}\"");
        Response.Headers.Add("Accept-Ranges", "bytes");
        Response.ContentType = fileDescriptor.MimeType;

        return await FileDescriptorAppService.DownloadAsync(id, token);
    }
}
