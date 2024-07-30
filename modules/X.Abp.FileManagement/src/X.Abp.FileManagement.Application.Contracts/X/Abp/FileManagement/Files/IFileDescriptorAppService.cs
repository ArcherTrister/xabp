// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace X.Abp.FileManagement.Files;

public interface IFileDescriptorAppService : IApplicationService
{
    Task<FileDescriptorDto> GetAsync(Guid id);

    Task<ListResultDto<FileDescriptorDto>> GetListAsync(Guid? directoryId);

    Task<FileDescriptorDto> RenameAsync(Guid id, RenameFileInput input);

    Task<IRemoteStreamContent> DownloadAsync(Guid id, string token);

    Task DeleteAsync(Guid id);

    Task<FileDescriptorDto> CreateAsync(Guid? directoryId, [NotNull] CreateFileInputWithStream inputWithStream);

    Task<FileDescriptorDto> MoveAsync(MoveFileInput input);

    Task<List<FileUploadPreInfoDto>> GetPreInfoAsync(List<FileUploadPreInfoRequest> input);

    Task<byte[]> GetContentAsync(Guid id);

    Task<DownloadTokenResultDto> GetDownloadTokenAsync(Guid id);
}
