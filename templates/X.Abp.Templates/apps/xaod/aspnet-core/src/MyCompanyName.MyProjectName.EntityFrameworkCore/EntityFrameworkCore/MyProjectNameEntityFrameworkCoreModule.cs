// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
#if MySQL
using Volo.Abp.EntityFrameworkCore.MySQL;
#elif SqlServer
using Volo.Abp.EntityFrameworkCore.SqlServer;
#elif Sqlite
using Volo.Abp.EntityFrameworkCore.Sqlite;
#elif Oracle
using Volo.Abp.EntityFrameworkCore.Oracle;
#elif OracleDevart
using Volo.Abp.EntityFrameworkCore.Oracle.Devart;
#elif PostgreSql
using Volo.Abp.EntityFrameworkCore.PostgreSql;
#endif
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;

using X.Abp.Gdpr;
using X.Abp.Identity.EntityFrameworkCore;
using X.Abp.OpenIddict.EntityFrameworkCore;
using X.Abp.LanguageManagement.EntityFrameworkCore;
using X.Abp.Saas.EntityFrameworkCore;
using X.Abp.TextTemplateManagement.EntityFrameworkCore;

namespace MyCompanyName.MyProjectName.EntityFrameworkCore;

[DependsOn(
    typeof(MyProjectNameDomainModule),
    typeof(AbpIdentityProEntityFrameworkCoreModule),
    typeof(AbpOpenIddictProEntityFrameworkCoreModule),
    typeof(AbpPermissionManagementEntityFrameworkCoreModule),
    typeof(AbpSettingManagementEntityFrameworkCoreModule),
#if MySQL
    typeof(AbpEntityFrameworkCoreMySQLModule),
#elif SqlServer
    typeof(AbpEntityFrameworkCoreSqlServerModule),
#elif Sqlite
    typeof(AbpEntityFrameworkCoreSqliteModule),
#elif Oracle
    typeof(AbpEntityFrameworkCoreOracleModule),
#elif OracleDevart
    typeof(AbpEntityFrameworkCoreOracleDevartModule),
#elif PostgreSql
    typeof(AbpEntityFrameworkCorePostgreSqlModule),
#endif
    typeof(AbpBackgroundJobsEntityFrameworkCoreModule),
    typeof(AbpAuditLoggingEntityFrameworkCoreModule),
    typeof(AbpFeatureManagementEntityFrameworkCoreModule),
    typeof(AbpLanguageManagementEntityFrameworkCoreModule),
    typeof(AbpSaasEntityFrameworkCoreModule),
    typeof(AbpTextTemplateManagementEntityFrameworkCoreModule),
    typeof(AbpGdprEntityFrameworkCoreModule),
    typeof(BlobStoringDatabaseEntityFrameworkCoreModule)
)]
public class MyProjectNameEntityFrameworkCoreModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        MyProjectNameEfCoreEntityExtensionMappings.Configure();
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<MyProjectNameDbContext>(options =>
        {
            /* Remove "includeAllEntities: true" to create
             * default repositories only for aggregate roots */
            options.AddDefaultRepositories(includeAllEntities: true);
        });

        Configure<AbpDbContextOptions>(options =>
        {
            /* The main point to change your DBMS.
             * See also MyProjectNameDbContextFactory for EF Core tooling. */
#if MySQL
            options.UseMySQL();
#elif SqlServer
            options.UseSqlServer();
#elif Sqlite
            options.UseSqlite();
#elif Oracle
            options.UseOracle();
#elif OracleDevart
            options.UseOracle();
#elif PostgreSql
            options.UseNpgsql();
#endif
        });
    }
}
