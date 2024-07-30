// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Data;
using Volo.Abp.Features;

using X.Abp.FileManagement.Files;
using X.Abp.FileManagement.Permissions;

namespace X.Abp.FileManagement.Directories;

[RequiresFeature(FileManagementFeatures.Enable)]
[Authorize(AbpFileManagementPermissions.DirectoryDescriptor.Default)]
public class DirectoryDescriptorAppService : FileManagementAppServiceBase, IDirectoryDescriptorAppService
{
    protected IDirectoryManager DirectoryManager { get; }

    protected IDirectoryDescriptorRepository DirectoryDescriptorRepository { get; }

    protected IFileManager FileManager { get; }

    protected IFileDescriptorRepository FileDescriptorRepository { get; }

    protected FileIconOption FileIconOption { get; }

    public DirectoryDescriptorAppService(
        IDirectoryManager directoryManager,
        IFileManager fileManager,
        IDirectoryDescriptorRepository directoryDescriptorRepository,
        IFileDescriptorRepository fileDescriptorRepository,
        IOptions<FileIconOption> fileIconOption)
    {
        DirectoryManager = directoryManager;
        FileManager = fileManager;
        DirectoryDescriptorRepository = directoryDescriptorRepository;
        FileDescriptorRepository = fileDescriptorRepository;

        FileIconOption = fileIconOption.Value;
    }

    public virtual async Task<DirectoryDescriptorDto> GetAsync(Guid id)
    {
        var directoryDescriptor = await DirectoryDescriptorRepository.GetAsync(id);

        return ObjectMapper.Map<DirectoryDescriptor, DirectoryDescriptorDto>(directoryDescriptor);
    }

    public virtual async Task<ListResultDto<DirectoryDescriptorInfoDto>> GetListAsync(Guid? parentId)
    {
        var subDirectories = await DirectoryDescriptorRepository.GetChildrenAsync(parentId);

        var result = new List<DirectoryDescriptorInfoDto>();

        foreach (var subDirectory in subDirectories)
        {
            result.Add(new DirectoryDescriptorInfoDto
            {
                Name = subDirectory.Name,
                Id = subDirectory.Id,
                ParentId = subDirectory.ParentId,
                HasChildren = await DirectoryDescriptorRepository.ContainsAnyAsync(subDirectory.Id, false)
            });
        }

        return new ListResultDto<DirectoryDescriptorInfoDto>(result);
    }

    [Authorize(AbpFileManagementPermissions.DirectoryDescriptor.Create)]
    public virtual async Task<DirectoryDescriptorDto> CreateAsync(CreateDirectoryInput input)
    {
        var directoryDescriptor = await DirectoryManager.CreateAsync(input.Name, input.ParentId, CurrentTenant.Id);

        await DirectoryDescriptorRepository.InsertAsync(directoryDescriptor);

        return ObjectMapper.Map<DirectoryDescriptor, DirectoryDescriptorDto>(directoryDescriptor);
    }

    [Authorize(AbpFileManagementPermissions.DirectoryDescriptor.Update)]
    public virtual async Task<DirectoryDescriptorDto> RenameAsync(Guid id, RenameDirectoryInput input)
    {
        var directory = await DirectoryDescriptorRepository.GetAsync(id);

        directory.SetConcurrencyStampIfNotNull(input.ConcurrencyStamp);

        await DirectoryManager.RenameAsync(directory, input.Name);
        await DirectoryDescriptorRepository.UpdateAsync(directory);

        return ObjectMapper.Map<DirectoryDescriptor, DirectoryDescriptorDto>(directory);
    }

    public virtual async Task<PagedResultDto<DirectoryContentDto>> GetContentAsync(DirectoryContentRequestInput input)
    {
        var result = new List<DirectoryContentDto>();

        // directory can be orderable for only its name
        var directorySorting =
                input.Sorting?.IndexOf("name asc", StringComparison.OrdinalIgnoreCase) >= 0 ?
                "name asc" :
                input.Sorting?.IndexOf("name desc", StringComparison.OrdinalIgnoreCase) >= 0 ?
                "name desc" :
                null;

        var subDirectories = await DirectoryDescriptorRepository.GetChildrenAsync(input.Id, input.Filter, directorySorting);
        result.AddRange(
            subDirectories.Select(
                x => new DirectoryContentDto
                {
                    Id = x.Id,
                    IsDirectory = true,
                    Name = x.Name,
                    Size = 0,
                    ConcurrencyStamp = x.ConcurrencyStamp,
                }));

        if (await AuthorizationService.IsGrantedAsync(AbpFileManagementPermissions.FileDescriptor.Default))
        {
            var subFiles = await FileDescriptorRepository.GetListAsync(input.Id, input.Filter, input.Sorting);
            result.AddRange(
                subFiles.Select(
                    x => new DirectoryContentDto
                    {
                        Id = x.Id,
                        IsDirectory = false,
                        Name = x.Name,
                        Size = x.Size,
                        IconInfo = FileIconOption.GetFileIconInfo(x.Name)
                    }));
        }

        var subDirectoryCount = await DirectoryDescriptorRepository.GetChildrenCountAsync(input.Id);
        var subFileCount = await FileDescriptorRepository.CountDirectoryFilesAsync(input.Id);

        return new PagedResultDto<DirectoryContentDto>(subDirectoryCount + subFileCount, result);
    }

    [Authorize(AbpFileManagementPermissions.DirectoryDescriptor.Delete)]
    public virtual async Task DeleteAsync(Guid id)
    {
        await DirectoryManager.DeleteAsync(id);
    }

    [Authorize(AbpFileManagementPermissions.DirectoryDescriptor.Update)]
    public virtual async Task<DirectoryDescriptorDto> MoveAsync(MoveDirectoryInput input)
    {
        var directory = await DirectoryDescriptorRepository.GetAsync(input.Id);

        directory.SetConcurrencyStampIfNotNull(input.ConcurrencyStamp);

        await DirectoryManager.MoveAsync(directory, input.NewParentId);
        await DirectoryDescriptorRepository.UpdateAsync(directory);

        return ObjectMapper.Map<DirectoryDescriptor, DirectoryDescriptorDto>(directory);
    }
}
