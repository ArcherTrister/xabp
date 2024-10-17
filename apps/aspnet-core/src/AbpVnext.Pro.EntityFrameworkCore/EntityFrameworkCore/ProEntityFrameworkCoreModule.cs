// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.MySQL;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;

using X.Abp.Chat.EntityFrameworkCore;
using X.Abp.CmsKit.EntityFrameworkCore;
using X.Abp.FileManagement.EntityFrameworkCore;
using X.Abp.Forms.EntityFrameworkCore;
using X.Abp.Gdpr;
using X.Abp.Identity.EntityFrameworkCore;
using X.Abp.IdentityServer.EntityFrameworkCore;
using X.Abp.LanguageManagement.EntityFrameworkCore;
using X.Abp.Notification.EntityFrameworkCore;
using X.Abp.OpenIddict.EntityFrameworkCore;
using X.Abp.Payment.EntityFrameworkCore;
using X.Abp.Saas.EntityFrameworkCore;
using X.Abp.TextTemplateManagement.EntityFrameworkCore;
using X.Abp.VersionManagement.EntityFrameworkCore;

namespace AbpVnext.Pro.EntityFrameworkCore;

[DependsOn(
    typeof(ProDomainModule),
    typeof(AbpIdentityProEntityFrameworkCoreModule),
    typeof(AbpOpenIddictProEntityFrameworkCoreModule),
    typeof(AbpIdentityServerProEntityFrameworkCoreModule),
    typeof(AbpPermissionManagementEntityFrameworkCoreModule),
    typeof(AbpSettingManagementEntityFrameworkCoreModule),
    typeof(AbpEntityFrameworkCoreMySQLModule),
    typeof(AbpBackgroundJobsEntityFrameworkCoreModule),
    typeof(AbpAuditLoggingEntityFrameworkCoreModule),
    typeof(AbpFeatureManagementEntityFrameworkCoreModule),
    typeof(AbpLanguageManagementEntityFrameworkCoreModule),
    typeof(AbpSaasEntityFrameworkCoreModule),
    typeof(AbpTextTemplateManagementEntityFrameworkCoreModule),
    typeof(AbpGdprEntityFrameworkCoreModule),
    typeof(BlobStoringDatabaseEntityFrameworkCoreModule))]

// [DependsOn(typeof(IotEntityFrameworkCoreModule))]
// [DependsOn(typeof(AbpBasicDataEntityFrameworkCoreModule))]
[DependsOn(typeof(AbpNotificationEntityFrameworkCoreModule))]
[DependsOn(typeof(AbpChatEntityFrameworkCoreModule))]
[DependsOn(typeof(AbpFileManagementEntityFrameworkCoreModule))]
[DependsOn(typeof(AbpFormsEntityFrameworkCoreModule))]
[DependsOn(typeof(AbpCmsKitProEntityFrameworkCoreModule))]
[DependsOn(typeof(AbpPaymentEntityFrameworkCoreModule))]
[DependsOn(typeof(AbpVersionManagementEntityFrameworkCoreModule))]
public class ProEntityFrameworkCoreModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        ProEfCoreEntityExtensionMappings.Configure();
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<ProDbContext>(options =>
        {
            /* Remove "includeAllEntities: true" to create
             * default repositories only for aggregate roots */
            options.AddDefaultRepositories(includeAllEntities: true);
        });

        context.Services.AddAbpDbContext<ProTenantDbContext>(options =>
        {
            /* Remove "includeAllEntities: true" to create
             * default repositories only for aggregate roots */
            options.AddDefaultRepositories(includeAllEntities: true);
        });

        Configure<AbpDbContextOptions>(options =>
        {
            /* The main point to change your DBMS.
             * See also ProDbContextFactoryBase for EF Core tooling. */
            options.UseMySQL();
        });
    }
}
