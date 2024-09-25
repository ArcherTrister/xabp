// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.EntityFrameworkCore;

using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

using X.Abp.TextTemplateManagement.TextTemplates;

namespace X.Abp.TextTemplateManagement.EntityFrameworkCore;

public static class TextTemplateManagementDbContextModelBuilderExtensions
{
    public static void ConfigureTextTemplateManagement(this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        builder.Entity<TextTemplateContent>(b =>
        {
            b.ToTable(TextTemplateManagementDbProperties.DbTablePrefix + "TextTemplateContents", TextTemplateManagementDbProperties.DbSchema);
            b.ConfigureByConvention();
            b.Property(textTemplateContent => textTemplateContent.Name).IsRequired().HasMaxLength(TextTemplateConsts.MaxNameLength);
            b.Property(textTemplateContent => textTemplateContent.CultureName).HasMaxLength(TextTemplateConsts.MaxCultureNameLength);
            b.Property(textTemplateContent => textTemplateContent.Content).IsRequired().HasMaxLength(TextTemplateConsts.MaxContentLength);
            b.ApplyObjectExtensionMappings();
        });

        if (builder.IsHostDatabase())
        {
            builder.Entity<TextTemplateDefinitionRecord>(b =>
            {
                b.ToTable(TextTemplateManagementDbProperties.DbTablePrefix + "TextTemplateDefinitionRecords", TextTemplateManagementDbProperties.DbSchema);
                b.ConfigureByConvention();
                b.Property(definitionRecord => definitionRecord.Name).HasMaxLength(TemplateDefinitionRecordConsts.MaxNameLength).IsRequired(true);
                b.Property(definitionRecord => definitionRecord.DisplayName).HasMaxLength(TemplateDefinitionRecordConsts.MaxDisplayNameLength);
                b.Property(definitionRecord => definitionRecord.Layout).HasMaxLength(TemplateDefinitionRecordConsts.MaxLayoutLength);
                b.Property(definitionRecord => definitionRecord.LocalizationResourceName).HasMaxLength(TemplateDefinitionRecordConsts.MaxLocalizationResourceNameLength);
                b.Property(definitionRecord => definitionRecord.DefaultCultureName).HasMaxLength(TemplateDefinitionRecordConsts.MaxDefaultCultureNameLength);
                b.Property(definitionRecord => definitionRecord.RenderEngine).HasMaxLength(TemplateDefinitionRecordConsts.MaxRenderEngineLength);
                b.HasMany<TextTemplateDefinitionContentRecord>().WithOne().HasForeignKey(definitionContentRecord => definitionContentRecord.DefinitionId);
                b.HasIndex(definitionRecord => new
                {
                    Name = definitionRecord.Name
                }).IsUnique(true);
                b.ApplyObjectExtensionMappings();
            });
            builder.Entity<TextTemplateDefinitionContentRecord>(b =>
            {
                b.ToTable(TextTemplateManagementDbProperties.DbTablePrefix + "TextTemplateDefinitionContentRecords", TextTemplateManagementDbProperties.DbSchema);
                b.ConfigureByConvention();
                b.Property(definitionContentRecord => definitionContentRecord.FileName).HasMaxLength(TemplateDefinitionContentRecordConsts.MaxFileNameLength).IsRequired(true);
                b.HasOne<TextTemplateDefinitionRecord>().WithMany().HasForeignKey(definitionContentRecord => definitionContentRecord.DefinitionId);
                b.ApplyObjectExtensionMappings();
            });
        }

        builder.TryConfigureObjectExtensions<TextTemplateManagementDbContext>();
    }
}
