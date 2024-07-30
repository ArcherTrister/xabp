// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.EntityFrameworkCore;

using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;

using X.Abp.Gdpr;
using X.Abp.Identity;
using X.Abp.Identity.EntityFrameworkCore;
using X.Abp.IdentityServer.EntityFrameworkCore;
using X.Abp.LanguageManagement.EntityFrameworkCore;
using X.Abp.Saas.Editions;
using X.Abp.Saas.EntityFrameworkCore;
using X.Abp.Saas.Tenants;
using X.Abp.TextTemplateManagement.EntityFrameworkCore;

namespace MyCompanyName.MyProjectName.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityProDbContext))]
[ReplaceDbContext(typeof(ISaasDbContext))]
[ConnectionStringName("Default")]
public class MyProjectNameDbContext : AbpDbContext<MyProjectNameDbContext>, IIdentityProDbContext, ISaasDbContext
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */

    #region Entities from the modules

    /* Notice: We only implemented IIdentityProDbContext and ISaasDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityProDbContext and ISaasDbContext.
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    // Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }

    // SaaS
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<Edition> Editions { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    #endregion

    public MyProjectNameDbContext(DbContextOptions<MyProjectNameDbContext> options)
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
        modelBuilder.ConfigureIdentityServerPro();
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
    }
}
