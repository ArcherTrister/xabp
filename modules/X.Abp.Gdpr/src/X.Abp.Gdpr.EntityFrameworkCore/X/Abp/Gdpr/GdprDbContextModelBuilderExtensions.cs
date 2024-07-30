// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.EntityFrameworkCore;

using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace X.Abp.Gdpr;

public static class GdprDbContextModelBuilderExtensions
{
    public static void ConfigureGdpr(this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));
        builder.Entity<GdprRequest>(b =>
        {
            b.ToTable(GdprDbProperties.DbTablePrefix + "Requests", GdprDbProperties.DbSchema);
            b.ConfigureByConvention();
            b.Property(gdprRequest => gdprRequest.UserId).IsRequired().HasColumnName("UserId");
            b.Property(gdprRequest => gdprRequest.ReadyTime).IsRequired().HasColumnName("ReadyTime");
            b.HasMany(gdprRequest => gdprRequest.Infos).WithOne().HasForeignKey(gdprInfo => gdprInfo.RequestId).IsRequired(true);
            b.HasIndex(gdprRequest => gdprRequest.UserId);
        });
        builder.Entity<GdprInfo>(b =>
        {
            b.ToTable(GdprDbProperties.DbTablePrefix + "Infos", GdprDbProperties.DbSchema);
            b.ConfigureByConvention();
            b.Property(gdprInfo => gdprInfo.Provider).IsRequired().HasColumnName("Provider").HasMaxLength(GdprInfoConsts.MaxProviderLength);
            b.Property(gdprInfo => gdprInfo.Data).IsRequired().HasColumnName("Data");
            b.HasIndex(gdprInfo => gdprInfo.RequestId);
        });

        builder.TryConfigureObjectExtensions<GdprDbContext>();
    }
}
