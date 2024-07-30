// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Volo.Abp.Domain.Repositories;

namespace X.Abp.FileManagement.Directories;

public interface IDirectoryDescriptorRepository : IBasicRepository<DirectoryDescriptor, Guid>
{
    Task<DirectoryDescriptor> FindAsync([NotNull] string name, Guid? parentId = null, CancellationToken cancellationToken = default);

    Task<int> GetChildrenCountAsync(Guid? parentId, CancellationToken cancellationToken = default);

    Task<bool> ContainsAnyAsync(Guid? id, bool checkFilesAlso = true, CancellationToken cancellationToken = default);

    Task<List<DirectoryDescriptor>> GetChildrenAsync(Guid? parentId, string filter = null, string sorting = null, CancellationToken cancellationToken = default);

    Task<List<Guid>> GetAllChildrenIdsAsync(Guid? id, CancellationToken cancellationToken = default);

    Task<List<DirectoryDescriptor>> GetAllChildrenAsync(Guid? id, CancellationToken cancellationToken = default);
}
