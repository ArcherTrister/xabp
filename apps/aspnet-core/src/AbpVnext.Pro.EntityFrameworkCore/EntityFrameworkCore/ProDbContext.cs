// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.EntityFrameworkCore;

using Volo.Abp.Data;
using Volo.Abp.MultiTenancy;

namespace AbpVnext.Pro.EntityFrameworkCore;

[ConnectionStringName("Default")]
public class ProDbContext : ProDbContextBase<ProDbContext>
{
    public ProDbContext(DbContextOptions<ProDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.SetMultiTenancySide(MultiTenancySides.Both);

        base.OnModelCreating(modelBuilder);
    }
}
