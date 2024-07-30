// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.EntityFrameworkCore;

using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace X.Abp.Gdpr;

[ConnectionStringName(GdprDbProperties.ConnectionStringName)]
public class GdprDbContext : AbpDbContext<GdprDbContext>, IGdprDbContext
{
    public DbSet<GdprRequest> Requests { get; set; }

    public GdprDbContext(DbContextOptions<GdprDbContext> options)
      : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ConfigureGdpr();
    }
}
