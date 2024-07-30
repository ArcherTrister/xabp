// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.EntityFrameworkCore;

using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
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

public abstract class ProDbContextBase<TDbContext> : AbpDbContext<TDbContext>
    where TDbContext : DbContext
{
    protected ProDbContextBase(DbContextOptions<TDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        /* Include modules to your migration db context */

        modelBuilder.ConfigurePermissionManagement();
        modelBuilder.ConfigureSettingManagement();
        modelBuilder.ConfigureBackgroundJobs();
        modelBuilder.ConfigureAuditLogging();
        modelBuilder.ConfigureIdentityPro();

        modelBuilder.ConfigureOpenIddictPro();
        modelBuilder.ConfigureIdentityServerPro();
        modelBuilder.ConfigureFeatureManagement();
        modelBuilder.ConfigureLanguageManagement();
        modelBuilder.ConfigureSaas();
        modelBuilder.ConfigureTextTemplateManagement();
        modelBuilder.ConfigureBlobStoring();
        modelBuilder.ConfigureGdpr();
        modelBuilder.ConfigureChat();
        modelBuilder.ConfigureFileManagement();
        modelBuilder.ConfigureForms();

        // modelBuilder.ConfigureIot();
        modelBuilder.ConfigureNotification();
        modelBuilder.ConfigureCmsKitPro();

        modelBuilder.ConfigurePayment();

        modelBuilder.ConfigureVersionManagement();

        //modelBuilder.ConfigureBasicData();

        /* Configure your own tables/entities inside here */

        // modelBuilder.Entity<YourEntity>(b =>
        // {
        //    b.ToTable(ProConsts.DbTablePrefix + "YourEntities", ProConsts.DbSchema);
        //    b.ConfigureByConvention(); //auto configure for the base class props
        //    //...
        // });

        // if (modelBuilder.IsHostDatabase())
        // {
        //    /* Tip: Configure mappings like that for the entities only available in the host side,
        //     * but should not be in the tenant databases. */
        // }
    }
}
