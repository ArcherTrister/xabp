// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.Modularity;
using Volo.Abp.MongoDB;

using X.Abp.LanguageManagement.External;
using X.Abp.LanguageManagement.MongoDB;

namespace X.Abp.LanguageManagement
{
    [DependsOn(
    typeof(AbpLanguageManagementDomainModule),
    typeof(AbpMongoDbModule))]
    public class AbpLanguageManagementMongoDbModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddMongoDbContext<LanguageManagementMongoDbContext>(options =>
            {
                /* Add custom repositories here. Example:
                 * options.AddRepository<Question, MongoQuestionRepository>();
                 */
                options.AddRepository<Language, MongoLanguageRepository>();
                options.AddRepository<LanguageText, MongoLanguageTextRepository>();
                options.AddRepository<LocalizationResourceRecord, MongoLocalizationResourceRecordRepository>();
                options.AddRepository<LocalizationTextRecord, MongoLocalizationTextRecordRepository>();
            });
        }
    }
}
