using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.Modularity;
using Volo.Abp.MongoDB;

namespace X.Abp.LanguageManagement
{
    [DependsOn(
    typeof(AbpLanguageManagementDomainModule),
    typeof(AbpMongoDbModule)
    )]
    public class AbpLanguageManagementMongoDbModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddMongoDbContext<LanguageManagementMongoDbContext>(options =>
            {
                /* Add custom repositories here. Example:
                 * options.AddRepository<Question, MongoQuestionRepository>();
                 */
            });
        }
    }
}
