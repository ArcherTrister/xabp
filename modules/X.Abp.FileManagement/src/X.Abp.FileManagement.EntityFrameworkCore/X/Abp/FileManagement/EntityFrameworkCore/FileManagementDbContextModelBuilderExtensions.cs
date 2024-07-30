// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.EntityFrameworkCore;

using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

using X.Abp.FileManagement.Directories;
using X.Abp.FileManagement.Files;

namespace X.Abp.FileManagement.EntityFrameworkCore;

public static class FileManagementDbContextModelBuilderExtensions
{
    public static void ConfigureFileManagement(this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        builder.Entity<DirectoryDescriptor>(b =>
        {
            b.ToTable(FileManagementDbProperties.DbTablePrefix + "DirectoryDescriptors", FileManagementDbProperties.DbSchema);

            b.ConfigureByConvention();

            b.Property(p => p.Name).IsRequired().HasMaxLength(DirectoryDescriptorConsts.MaxNameLength);

            b.HasMany<DirectoryDescriptor>().WithOne().HasForeignKey(p => p.ParentId);
            b.HasOne<DirectoryDescriptor>().WithMany().HasForeignKey(p => p.ParentId);

            b.HasMany<FileDescriptor>().WithOne().HasForeignKey(p => p.DirectoryId);

            b.HasIndex(x => new { x.TenantId, x.ParentId, x.Name });

            b.ApplyObjectExtensionMappings();
        });

        builder.Entity<FileDescriptor>(b =>
        {
            b.ToTable(FileManagementDbProperties.DbTablePrefix + "FileDescriptors", FileManagementDbProperties.DbSchema);

            b.ConfigureByConvention();

            b.Property(p => p.Name).IsRequired().HasMaxLength(FileDescriptorConsts.MaxNameLength);
            b.Property(p => p.UniqueFileName).IsRequired().HasMaxLength(FileDescriptorConsts.MaxUniqueFileNameLength);
            b.Property(p => p.MimeType).IsRequired().HasMaxLength(FileDescriptorConsts.MaxMimeTypeLength);
            b.Property(p => p.Size).IsRequired().HasMaxLength(FileDescriptorConsts.MaxSizeLength);

            b.HasOne<DirectoryDescriptor>().WithMany().HasForeignKey(p => p.DirectoryId);

            b.HasIndex(x => new { x.TenantId, x.DirectoryId, x.Name });

            b.ApplyObjectExtensionMappings();
        });

        builder.TryConfigureObjectExtensions<FileManagementDbContext>();
    }
}
