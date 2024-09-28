// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Volo.Abp.Content;
using Volo.Abp.Domain.Services;

namespace X.Abp.FileManagement.Files;

public interface IFileManager : IDomainService
{
    Task<FileDescriptor> CreateAsync(
        [NotNull] string name,
        [NotNull] string mimeType,
        [NotNull] IRemoteStreamContent content,
        Guid? directoryId = null,
        Guid? tenantId = null,
        bool overrideExisting = false);

    Task RenameAsync(FileDescriptor file, [NotNull] string newName);

    Task DeleteAllAsync(Guid? directoryId);

    Task DeleteAsync(FileDescriptor file);

    Task MoveAsync(FileDescriptor file, Guid? newDirectoryId);
}
