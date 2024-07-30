// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

using X.Abp.LanguageManagement.External;

namespace X.Abp.LanguageManagement.EntityFrameworkCore;

[DependsOn(
    typeof(AbpLanguageManagementDomainModule),
    typeof(AbpEntityFrameworkCoreModule))]
public class AbpLanguageManagementEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<LanguageManagementDbContext>(options =>
        {
            options.AddDefaultRepositories<ILanguageManagementDbContext>();

            options.AddRepository<Language, EfCoreLanguageRepository>();
            options.AddRepository<LanguageText, EfCoreLanguageTextRepository>();
            options.AddRepository<LocalizationResourceRecord, EfCoreLocalizationResourceRecordRepository>();
            options.AddRepository<LocalizationTextRecord, EfCoreLocalizationTextRecordRepository>();
        });
    }
}
