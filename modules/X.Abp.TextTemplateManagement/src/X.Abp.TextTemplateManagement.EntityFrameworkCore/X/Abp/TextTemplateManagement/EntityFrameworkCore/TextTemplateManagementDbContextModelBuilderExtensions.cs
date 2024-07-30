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

        builder.TryConfigureObjectExtensions<TextTemplateManagementDbContext>();
    }
}
