// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using MongoDB.Driver;

using Volo.Abp.Data;
using Volo.Abp.MongoDB;

using X.Abp.LanguageManagement.External;

namespace X.Abp.LanguageManagement;

[ConnectionStringName(LanguageManagementDbProperties.ConnectionStringName)]
public interface ILanguageManagementMongoDbContext : IAbpMongoDbContext
{
    /* Define mongo collections here. Example:
     * IMongoCollection<Question> Questions { get; }
     */
    IMongoCollection<Language> Languages { get; }

    IMongoCollection<LanguageText> LanguageTexts { get; }

    IMongoCollection<LocalizationResourceRecord> LocalizationResources { get; }

    IMongoCollection<LocalizationTextRecord> LocalizationTexts { get; }
}
