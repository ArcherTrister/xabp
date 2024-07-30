// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.EntityFrameworkCore;

using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;

using X.Abp.Gdpr;
using X.Abp.Identity.EntityFrameworkCore;
using X.Abp.LanguageManagement.EntityFrameworkCore;
using X.Abp.OpenIddict.EntityFrameworkCore;
using X.Abp.TextTemplateManagement.EntityFrameworkCore;
using X.Abp.Saas.EntityFrameworkCore;

namespace MyCompanyName.MyProjectName.EntityFrameworkCore
{
    public abstract class MyProjectNameDbContextBase<TDbContext> : AbpDbContext<TDbContext>
        where TDbContext : DbContext
    {
        public MyProjectNameDbContextBase(DbContextOptions<TDbContext> options)
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
            modelBuilder.ConfigureFeatureManagement();
            modelBuilder.ConfigureLanguageManagement();
            modelBuilder.ConfigureSaas();
            modelBuilder.ConfigureTextTemplateManagement();
            modelBuilder.ConfigureBlobStoring();
            modelBuilder.ConfigureGdpr();

            /* Configure your own tables/entities inside here */

            //modelBuilder.Entity<YourEntity>(b =>
            //{
            //    b.ToTable(MyProjectNameConsts.DbTablePrefix + "YourEntities", MyProjectNameConsts.DbSchema);
            //    b.ConfigureByConvention(); //auto configure for the base class props
            //    //...
            //});

            //if (modelBuilder.IsHostDatabase())
            //{
            //    /* Tip: Configure mappings like that for the entities only available in the host side,
            //     * but should not be in the tenant databases. */
            //}
        }
    }
}
