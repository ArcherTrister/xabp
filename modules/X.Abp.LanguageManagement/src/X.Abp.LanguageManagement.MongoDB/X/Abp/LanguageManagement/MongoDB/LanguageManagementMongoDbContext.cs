// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using MongoDB.Driver;

using Volo.Abp.Data;
using Volo.Abp.MongoDB;

using X.Abp.LanguageManagement.External;

namespace X.Abp.LanguageManagement;

[ConnectionStringName(LanguageManagementDbProperties.ConnectionStringName)]
public class LanguageManagementMongoDbContext : AbpMongoDbContext, ILanguageManagementMongoDbContext
{
    /* Add mongo collections here. Example:
     * public IMongoCollection<Question> Questions => Collection<Question>();
     */
    public IMongoCollection<Language> Languages => Collection<Language>();

    public IMongoCollection<LanguageText> LanguageTexts => Collection<LanguageText>();

    public IMongoCollection<LocalizationResourceRecord> LocalizationResources => Collection<LocalizationResourceRecord>();

    public IMongoCollection<LocalizationTextRecord> LocalizationTexts => Collection<LocalizationTextRecord>();

    protected override void CreateModel(IMongoModelBuilder modelBuilder)
    {
        base.CreateModel(modelBuilder);

        modelBuilder.ConfigureLanguageManagement();
    }
}
