// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.EntityFrameworkCore;

using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

using X.Abp.Saas.Editions;
using X.Abp.Saas.Tenants;

namespace X.Abp.Saas.EntityFrameworkCore;

public static class SaasDbContextModelBuilderExtensions
{
    public static void ConfigureSaas(this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));
        if (builder.IsTenantOnlyDatabase())
        {
            return;
        }

        builder.Entity<Tenant>(b =>
        {
            b.ToTable(SaasDbProperties.DbTablePrefix + "Tenants", SaasDbProperties.DbSchema);
            b.ConfigureByConvention();
            b.Property(tenant => tenant.Name).IsRequired().HasMaxLength(TenantConsts.MaxNameLength).HasColumnName(nameof(Tenant.Name));
            b.HasMany(tenant => tenant.ConnectionStrings).WithOne().HasForeignKey(connectionString => connectionString.TenantId).IsRequired(true);
            b.HasIndex(tenant => tenant.Name);
            b.ApplyObjectExtensionMappings();
        });

        builder.Entity<Edition>(b =>
        {
            b.ToTable(SaasDbProperties.DbTablePrefix + "Editions", SaasDbProperties.DbSchema);
            b.ConfigureByConvention();
            b.Property(edition => edition.DisplayName).IsRequired().HasMaxLength(EditionConsts.MaxDisplayNameLength).HasColumnName(nameof(Edition.DisplayName));
            b.HasIndex(edition => edition.DisplayName);
            b.ApplyObjectExtensionMappings();
        });

        builder.Entity<TenantConnectionString>(b =>
        {
            b.ToTable(SaasDbProperties.DbTablePrefix + "TenantConnectionStrings", SaasDbProperties.DbSchema);
            b.ConfigureByConvention();
            b.HasKey(connectionString => new
            {
                connectionString.TenantId,
                connectionString.Name
            });
            b.Property(connectionString => connectionString.Name).IsRequired().HasMaxLength(TenantConnectionStringConsts.MaxNameLength).HasColumnName(nameof(TenantConnectionString.Name));
            b.Property(connectionString => connectionString.Value).IsRequired().HasMaxLength(TenantConnectionStringConsts.MaxValueLength).HasColumnName(nameof(TenantConnectionString.Value));
            b.ApplyObjectExtensionMappings();
        });

        builder.TryConfigureObjectExtensions<SaasDbContext>();
    }
}
