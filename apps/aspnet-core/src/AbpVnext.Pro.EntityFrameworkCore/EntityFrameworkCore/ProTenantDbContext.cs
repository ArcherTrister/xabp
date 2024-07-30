// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.EntityFrameworkCore;

using Volo.Abp.Data;
using Volo.Abp.MultiTenancy;

namespace AbpVnext.Pro.EntityFrameworkCore;

[ConnectionStringName("Default")]
public class ProTenantDbContext : ProDbContextBase<ProTenantDbContext>
{
    public ProTenantDbContext(DbContextOptions<ProTenantDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.SetMultiTenancySide(MultiTenancySides.Tenant);

        base.OnModelCreating(modelBuilder);
    }
}
