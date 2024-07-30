// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.IO;

using JetBrains.Annotations;

using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace X.Abp.FileManagement.Files;

public class FileDescriptor : AuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; protected set; }

    public Guid? DirectoryId { get; protected set; }

    public string Name { get; protected set; }

    public string UniqueFileName { get; protected set; }

    public string MimeType { get; protected set; }

    public int Size { get; protected set; }

    protected FileDescriptor()
    {
    }

    public FileDescriptor(
        Guid id,
        [NotNull] string name,
        [NotNull] string mimeType,
        Guid? directoryId = null,
        int size = 0,
        Guid? tenantId = null)
        : base(id)
    {
        TenantId = tenantId;
        DirectoryId = directoryId;
        MimeType = mimeType;
        Size = size;

        SetName(name);
        UniqueFileName = GenerateUniqueFileName();
    }

    internal virtual void SetName([NotNull] string name)
    {
        Name = FileNameValidator.CheckFileName(name);
    }

    internal virtual void SetSize(int size)
    {
        Size = size;
    }

    internal virtual void SetDirectoryId(Guid? directoryId)
    {
        DirectoryId = directoryId;
    }

    protected virtual string GenerateUniqueFileName()
    {
        return $"{Id}{Path.GetExtension(Name)}";
    }
}
