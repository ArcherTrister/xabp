using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.SqlServer;
using Volo.Abp.Modularity;

using X.Abp.Payment;
using X.Abp.Payment.EntityFrameworkCore;
using X.Abp.Saas.EntityFrameworkCore;

namespace MyCompanyName.MyProjectName.SaasService.EntityFrameworkCore;

[DependsOn(
    typeof(SaasServiceDomainModule),
    typeof(AbpSaasEntityFrameworkCoreModule),
    typeof(AbpEntityFrameworkCoreSqlServerModule),
    typeof(AbpPaymentEntityFrameworkCoreModule))]
public class SaasServiceEntityFrameworkCoreModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        SaasServiceEfCoreEntityExtensionMappings.Configure();
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<SaasServiceDbContext>(options =>
        {
            options.ReplaceDbContext<ISaasDbContext>();
            options.ReplaceDbContext<IPaymentDbContext>();

                /* includeAllEntities: true allows to use IRepository<TEntity, TKey> also for non aggregate root entities */
            options.AddDefaultRepositories(includeAllEntities: true);
        });

        Configure<AbpDbContextOptions>(options =>
        {
            options.Configure<SaasServiceDbContext>(c =>
            {
                c.UseSqlServer(b =>
                {
                    b.MigrationsHistoryTable("__SaasService_Migrations");
                });
            });
        });
    }
}
