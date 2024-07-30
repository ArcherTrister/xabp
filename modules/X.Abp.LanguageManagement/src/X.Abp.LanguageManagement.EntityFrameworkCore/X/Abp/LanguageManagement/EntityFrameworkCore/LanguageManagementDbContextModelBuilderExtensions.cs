// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.EntityFrameworkCore;

using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

using X.Abp.LanguageManagement.External;

namespace X.Abp.LanguageManagement.EntityFrameworkCore;

public static class LanguageManagementDbContextModelBuilderExtensions
{
    public static void ConfigureLanguageManagement(
        this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        if (builder.IsHostDatabase())
        {
            builder.Entity<Language>(b =>
            {
                b.ToTable(LanguageManagementDbProperties.DbTablePrefix + "Languages", LanguageManagementDbProperties.DbSchema);

                b.ConfigureByConvention();

                b.Property(x => x.CultureName).IsRequired().HasColumnName(nameof(Language.CultureName)).HasMaxLength(LanguageConsts.MaxCultureNameLength);
                b.Property(x => x.UiCultureName).IsRequired().HasColumnName(nameof(Language.UiCultureName)).HasMaxLength(LanguageConsts.MaxUiCultureNameLength);
                b.Property(x => x.DisplayName).IsRequired().HasColumnName(nameof(Language.DisplayName)).HasMaxLength(LanguageConsts.MaxDisplayNameLength);
                b.Property(x => x.FlagIcon).IsRequired(false).HasColumnName(nameof(Language.FlagIcon)).HasMaxLength(LanguageConsts.MaxFlagIconLength);
                b.Property(x => x.IsEnabled).IsRequired().HasColumnName(nameof(Language.IsEnabled));

                b.ApplyObjectExtensionMappings();
            });
        }

        builder.Entity<LanguageText>(b =>
        {
            b.ToTable(LanguageManagementDbProperties.DbTablePrefix + "LanguageTexts", LanguageManagementDbProperties.DbSchema);

            b.ConfigureByConvention();

            b.Property(x => x.ResourceName).IsRequired().HasColumnName(nameof(LanguageText.ResourceName)).HasMaxLength(LanguageTextConsts.MaxResourceNameLength);
            b.Property(x => x.Name).IsRequired().HasColumnName(nameof(LanguageText.Name)).HasMaxLength(LanguageTextConsts.MaxKeyNameLength);
            b.Property(x => x.Value).IsRequired().HasColumnName(nameof(LanguageText.Value)).HasMaxLength(LanguageTextConsts.MaxValueLength);
            b.Property(x => x.CultureName).IsRequired().HasColumnName(nameof(LanguageText.CultureName)).HasMaxLength(LanguageTextConsts.MaxCultureNameLength);

            b.HasIndex(x => new { x.TenantId, x.ResourceName, x.CultureName });

            b.ApplyObjectExtensionMappings();
        });

        builder.Entity<LocalizationResourceRecord>(b =>
        {
            b.ToTable(LanguageManagementDbProperties.DbTablePrefix + "LocalizationResourceRecords", LanguageManagementDbProperties.DbSchema);
            b.ConfigureByConvention();
            b.Property(l => l.Name).IsRequired().HasColumnName(nameof(LocalizationResourceRecord.Name)).HasMaxLength(LocalizationResourceRecordConsts.MaxNameLength);
            b.Property(l => l.DefaultCulture).HasColumnName(nameof(LocalizationResourceRecord.DefaultCulture)).HasMaxLength(LocalizationResourceRecordConsts.MaxDefaultCultureLength);
            b.Property(l => l.BaseResources).HasColumnName(nameof(LocalizationResourceRecord.BaseResources)).HasMaxLength(LocalizationResourceRecordConsts.MaxBaseResourcesLength);
            b.Property(l => l.SupportedCultures).HasColumnName(nameof(LocalizationResourceRecord.SupportedCultures)).HasMaxLength(LocalizationResourceRecordConsts.MaxSupportedCulturesLength);

            b.HasIndex(l => new { l.Name }).IsUnique();
        });

        builder.Entity<LocalizationTextRecord>(b =>
        {
            b.ToTable(LanguageManagementDbProperties.DbTablePrefix + "LocalizationTextRecords", LanguageManagementDbProperties.DbSchema);
            b.ConfigureByConvention();
            b.Property(l => l.ResourceName).IsRequired().HasColumnName(nameof(LocalizationTextRecord.ResourceName)).HasMaxLength(LocalizationTextRecordConsts.MaxResourceNameLength);
            b.Property(l => l.CultureName).IsRequired().HasColumnName(nameof(LocalizationTextRecord.CultureName)).HasMaxLength(LocalizationTextRecordConsts.MaxCultureNameLength);
            b.Property(l => l.Value).HasColumnName(nameof(LocalizationTextRecord.Value)).HasMaxLength(LocalizationTextRecordConsts.MaxValueLength);

            b.HasIndex(l => new { l.ResourceName, l.CultureName }).IsUnique();
        });

        builder.TryConfigureObjectExtensions<LanguageManagementDbContext>();
    }
}
