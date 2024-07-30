// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace X.Abp.FileManagement.Directories;

public interface IDirectoryDescriptorAppService : IApplicationService
{
    Task<DirectoryDescriptorDto> GetAsync(Guid id);

    Task<ListResultDto<DirectoryDescriptorInfoDto>> GetListAsync(Guid? parentId);

    Task<DirectoryDescriptorDto> CreateAsync(CreateDirectoryInput input);

    Task<DirectoryDescriptorDto> RenameAsync(Guid id, RenameDirectoryInput input);

    Task<PagedResultDto<DirectoryContentDto>> GetContentAsync(DirectoryContentRequestInput input);

    Task DeleteAsync(Guid id);

    Task<DirectoryDescriptorDto> MoveAsync(MoveDirectoryInput input);
}
