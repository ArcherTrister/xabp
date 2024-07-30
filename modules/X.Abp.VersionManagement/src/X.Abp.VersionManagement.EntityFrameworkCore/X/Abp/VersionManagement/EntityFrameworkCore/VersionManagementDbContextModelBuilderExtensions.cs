// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.EntityFrameworkCore;

using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

using X.Abp.VersionManagement.AppEditions;

namespace X.Abp.VersionManagement.EntityFrameworkCore;

public static class VersionManagementDbContextModelBuilderExtensions
{
    public static void ConfigureVersionManagement(
        this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        /* Configure all entities here. Example:

        builder.Entity<Question>(b =>
        {
            //Configure table & schema name
            b.ToTable(VersionManagementDbProperties.DbTablePrefix + "Questions", VersionManagementDbProperties.DbSchema);

            b.ConfigureByConvention();

            //Properties
            b.Property(q => q.Title).IsRequired().HasMaxLength(QuestionConsts.MaxTitleLength);

            //Relations
            b.HasMany(question => question.Tags).WithOne().HasForeignKey(qt => qt.QuestionId);

            //Indexes
            b.HasIndex(q => q.CreationTime);
        });
        */

        builder.Entity<AppEdition>(b =>
        {
            // Configure table & schema name
            b.ToTable(VersionManagementDbProperties.DbTablePrefix + "AppEditions", VersionManagementDbProperties.DbSchema);

            b.ConfigureByConvention();

            // Properties
            b.Property(q => q.AppName).IsRequired().HasMaxLength(20);
            b.Property(q => q.UniqueFileName).IsRequired().HasMaxLength(64);
            b.Property(q => q.FileExt).IsRequired().HasMaxLength(5);
            b.Property(q => q.MimeType).IsRequired().HasMaxLength(128);
            b.Property(q => q.Hash).IsRequired().HasMaxLength(200);
            b.Property(q => q.Version).IsRequired().HasMaxLength(20);
            b.Property(q => q.Channel).IsRequired().HasMaxLength(20);
            b.Property(q => q.Arch).IsRequired().HasMaxLength(20);
            b.Property(q => q.DownloadPath).IsRequired().HasMaxLength(200);
            b.Property(q => q.ReleaseDate).HasMaxLength(30);
            b.Property(q => q.UpdateContent).HasMaxLength(500);

            // Relations
            // b.HasMany(question => question.Tags).WithOne().HasForeignKey(qt => qt.QuestionId);

            // Indexes
            b.HasIndex(q => q.AppName);
            b.HasIndex(q => q.UniqueFileName);
        });

        builder.TryConfigureObjectExtensions<VersionManagementDbContext>();
    }
}
