// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.EntityFrameworkCore;

using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.MultiTenancy;

using X.Abp.Saas.Editions;
using X.Abp.Saas.Tenants;

namespace X.Abp.Saas.EntityFrameworkCore;

[ConnectionStringName(SaasDbProperties.ConnectionStringName)]
[IgnoreMultiTenancy]
public class SaasDbContext : AbpDbContext<SaasDbContext>, ISaasDbContext
{
    public DbSet<Tenant> Tenants { get; set; }

    public DbSet<Edition> Editions { get; set; }

    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    public SaasDbContext(DbContextOptions<SaasDbContext> options)
      : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ConfigureSaas();
    }
}
