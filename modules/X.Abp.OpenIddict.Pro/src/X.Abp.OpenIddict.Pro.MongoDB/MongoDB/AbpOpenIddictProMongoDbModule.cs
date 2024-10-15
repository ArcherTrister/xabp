using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.Modularity;
using Volo.Abp.MongoDB;

namespace X.Abp.OpenIddict.MongoDB
{
    [DependsOn(
    typeof(AbpOpenIddictProDomainModule),
    typeof(AbpMongoDbModule)
    )]
    public class AbpOpenIddictProMongoDbModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddMongoDbContext<OpenIddictProMongoDbContext>(options =>
            {
                /* Add custom repositories here. Example:
                 * options.AddRepository<Question, MongoQuestionRepository>();
                 */
            });
        }
    }
}
