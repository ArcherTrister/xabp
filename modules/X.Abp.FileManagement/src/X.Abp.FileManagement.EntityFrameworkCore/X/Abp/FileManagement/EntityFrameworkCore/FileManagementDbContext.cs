// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.EntityFrameworkCore;

using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

using X.Abp.FileManagement.Directories;
using X.Abp.FileManagement.Files;

namespace X.Abp.FileManagement.EntityFrameworkCore;

[ConnectionStringName(FileManagementDbProperties.ConnectionStringName)]
public class FileManagementDbContext : AbpDbContext<FileManagementDbContext>, IFileManagementDbContext
{
    public DbSet<DirectoryDescriptor> DirectoryDescriptions { get; set; }

    public DbSet<FileDescriptor> FileDescriptions { get; set; }

    public FileManagementDbContext(DbContextOptions<FileManagementDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ConfigureFileManagement();
    }
}
