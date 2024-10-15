using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace X.Abp.Quartz
{
    [DependsOn(
    typeof(AbpQuartzDomainModule),
    typeof(AbpEntityFrameworkCoreModule))]
    public class AbpQuartzEntityFrameworkCoreModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<QuartzDbContext>(options =>
            {
                /* Add custom repositories here. Example:
                 * options.AddRepository<Question, EfCoreQuestionRepository>();
                 */
                options.AddDefaultRepositories<IQuartzDbContext>();
            });
        }
    }
}
