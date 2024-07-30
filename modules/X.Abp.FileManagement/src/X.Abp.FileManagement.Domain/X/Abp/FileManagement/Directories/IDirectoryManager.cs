// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Volo.Abp.Domain.Services;

namespace X.Abp.FileManagement.Directories;

public interface IDirectoryManager : IDomainService
{
    Task<DirectoryDescriptor> CreateAsync([NotNull] string name, Guid? parentId = null, Guid? tenantId = null);

    Task RenameAsync(DirectoryDescriptor directory, string newName);

    Task MoveAsync(DirectoryDescriptor directory, Guid? newParentId);

    Task DeleteAsync(Guid id);
}
