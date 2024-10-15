// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.Identity;
using Volo.Abp.Identity.MongoDB;
using Volo.Abp.Modularity;

namespace X.Abp.Identity.MongoDB
{
    [DependsOn(
    typeof(AbpIdentityProDomainModule),
    typeof(AbpIdentityMongoDbModule)
    )]
    public class AbpIdentityProMongoDbModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddMongoDbContext<IdentityProMongoDbContext>(options =>
            {
                /* Add custom repositories here. Example:
                 * options.AddRepository<Question, MongoQuestionRepository>();
                 */
                options.ReplaceDbContext<IAbpIdentityMongoDbContext, IIdentityProMongoDbContext>();
                options.AddRepository<IdentityUser, MongoIdentityUserRepository>();
                options.AddRepository<IdentityRole, MongoIdentityRoleRepository>();
            });
        }
    }
}
