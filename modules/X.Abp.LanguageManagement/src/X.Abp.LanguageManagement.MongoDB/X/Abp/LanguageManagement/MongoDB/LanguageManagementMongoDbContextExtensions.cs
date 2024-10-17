// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp;
using Volo.Abp.MongoDB;

using X.Abp.LanguageManagement.External;

namespace X.Abp.LanguageManagement;

public static class LanguageManagementMongoDbContextExtensions
{
    public static void ConfigureLanguageManagement(this IMongoModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        builder.Entity<Language>(b => b.CollectionName = LanguageManagementDbProperties.DbTablePrefix + "Languages");
        builder.Entity<LanguageText>(b => b.CollectionName = LanguageManagementDbProperties.DbTablePrefix + "LanguageTexts");
        builder.Entity<LocalizationResourceRecord>(b => b.CollectionName = LanguageManagementDbProperties.DbTablePrefix + "LocalizationResourceRecords");
        builder.Entity<LocalizationTextRecord>(b => b.CollectionName = LanguageManagementDbProperties.DbTablePrefix + "LocalizationTextRecords");
    }
}
