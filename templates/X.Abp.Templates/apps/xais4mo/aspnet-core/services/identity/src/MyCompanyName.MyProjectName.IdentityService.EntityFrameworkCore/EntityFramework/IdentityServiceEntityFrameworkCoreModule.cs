using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.SqlServer;
using Volo.Abp.Modularity;

using X.Abp.Identity.EntityFrameworkCore;
using X.Abp.IdentityServer.EntityFrameworkCore;

namespace MyCompanyName.MyProjectName.IdentityService.EntityFramework;

[DependsOn(
    typeof(IdentityServiceDomainModule),
    typeof(AbpIdentityProEntityFrameworkCoreModule),
    typeof(AbpIdentityServerProEntityFrameworkCoreModule),
    typeof(AbpEntityFrameworkCoreSqlServerModule)
)]
public class IdentityServiceEntityFrameworkCoreModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        IdentityServiceEfCoreEntityExtensionMappings.Configure();
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<IdentityServiceDbContext>(options =>
        {
            options.ReplaceDbContext<IIdentityProDbContext>();
            options.ReplaceDbContext<IIdentityServerProDbContext>();

                /* includeAllEntities: true allows to use IRepository<TEntity, TKey> also for non aggregate root entities */
            options.AddDefaultRepositories(includeAllEntities: true);
        });

        Configure<AbpDbContextOptions>(options =>
        {
            options.Configure<IdentityServiceDbContext>(c =>
            {
                c.UseSqlServer(b =>
                {
                    b.MigrationsHistoryTable("__IdentityService_Migrations");
                });
            });
        });
    }
}
