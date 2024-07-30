// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using JetBrains.Annotations;

using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace X.Abp.FileManagement.Directories;

public class DirectoryDescriptor : AuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; protected set; }

    public string Name { get; protected set; }

    public Guid? ParentId { get; protected set; }

    protected DirectoryDescriptor()
    {
    }

    public DirectoryDescriptor(Guid id, [NotNull] string name, Guid? parentId = null, Guid? tenantId = null)
        : base(id)
    {
        TenantId = tenantId;
        ParentId = parentId;

        SetName(name);
    }

    internal virtual void SetName([NotNull] string name)
    {
        Name = FileNameValidator.CheckDirectoryName(name);
    }

    internal virtual void SetParentId(Guid? parentId)
    {
        ParentId = parentId;
    }
}
