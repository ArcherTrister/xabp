// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.EntityFrameworkCore;

using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

using X.Abp.VersionManagement.AppEditions;

namespace X.Abp.VersionManagement.EntityFrameworkCore;

[ConnectionStringName(VersionManagementDbProperties.ConnectionStringName)]
public class VersionManagementDbContext : AbpDbContext<VersionManagementDbContext>, IVersionManagementDbContext
{
    /* Add DbSet for each Aggregate Root here. Example:
     * public DbSet<Question> Questions { get; set; }
     */

    public DbSet<AppEdition> AppEditions { get; set; }

    public VersionManagementDbContext(DbContextOptions<VersionManagementDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ConfigureVersionManagement();
    }
}
