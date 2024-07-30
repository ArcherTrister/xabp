// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

using X.Abp.VersionManagement.AppEditions;

namespace X.Abp.VersionManagement.EntityFrameworkCore;

[DependsOn(
    typeof(AbpVersionManagementDomainModule),
    typeof(AbpEntityFrameworkCoreModule))]
public class AbpVersionManagementEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<VersionManagementDbContext>(options =>
        {
            /* Add custom repositories here. Example:
             * options.AddRepository<Question, EfCoreQuestionRepository>();
             */
            options.AddDefaultRepositories<IVersionManagementDbContext>();

            options.AddRepository<AppEdition, EfCoreAppEditionRepository>();
        });
    }
}
