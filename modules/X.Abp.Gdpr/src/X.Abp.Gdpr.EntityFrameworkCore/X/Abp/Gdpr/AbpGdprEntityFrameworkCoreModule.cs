// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace X.Abp.Gdpr;

[DependsOn(
    typeof(AbpGdprDomainModule),
    typeof(AbpEntityFrameworkCoreModule))]
public class AbpGdprEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<GdprDbContext>(options =>
        {
            /* Add custom repositories here. Example:
             * options.AddRepository<Question, EfCoreQuestionRepository>();
             */
            options.AddDefaultRepositories<IGdprDbContext>();

            options.AddRepository<GdprRequest, EfCoreGdprRequestRepository>();
        });
    }
}
