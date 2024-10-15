﻿using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.Modularity;
using Volo.Abp.MongoDB;

using X.Abp.TextTemplateManagement;

namespace X.Abp.TextTemplateManagement.MongoDB
{
    [DependsOn(
    typeof(AbpTextTemplateManagementDomainModule),
    typeof(AbpMongoDbModule)
    )]
    public class AbpTextTemplateManagementMongoDbModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddMongoDbContext<TextTemplateManagementMongoDbContext>(options =>
            {
                /* Add custom repositories here. Example:
                 * options.AddRepository<Question, MongoQuestionRepository>();
                 */
            });
        }
    }
}
