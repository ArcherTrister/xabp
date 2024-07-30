using Microsoft.EntityFrameworkCore;

using Volo.Abp.AuditLogging;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.FeatureManagement;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.PermissionManagement;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement;
using Volo.Abp.SettingManagement.EntityFrameworkCore;

using X.Abp.LanguageManagement;
using X.Abp.LanguageManagement.EntityFrameworkCore;
using X.Abp.TextTemplateManagement.EntityFrameworkCore;
using X.Abp.TextTemplateManagement.TextTemplates;

namespace MyCompanyName.MyProjectName.AdministrationService.EntityFrameworkCore;

[ConnectionStringName(AdministrationServiceDbProperties.ConnectionStringName)]
public class AdministrationServiceDbContext : AbpDbContext<AdministrationServiceDbContext>,
    IPermissionManagementDbContext,
    ISettingManagementDbContext,
    IFeatureManagementDbContext,
    IAuditLoggingDbContext,
    ILanguageManagementDbContext,
    ITextTemplateManagementDbContext,
    IBlobStoringDbContext
{
    public DbSet<PermissionGroupDefinitionRecord> PermissionGroups { get; set; }

    public DbSet<PermissionDefinitionRecord> Permissions { get; set; }

    public DbSet<PermissionGrant> PermissionGrants { get; set; }

    public DbSet<Setting> Settings { get; set; }

    public DbSet<FeatureGroupDefinitionRecord> FeatureGroups { get; set; }

    public DbSet<FeatureDefinitionRecord> Features { get; set; }

    public DbSet<FeatureValue> FeatureValues { get; set; }

    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<Language> Languages { get; set; }
    public DbSet<LanguageText> LanguageTexts { get; set; }
    public DbSet<TextTemplateContent> TextTemplateContents { get; set; }
    public DbSet<DatabaseBlobContainer> BlobContainers { get; set; }
    public DbSet<DatabaseBlob> Blobs { get; set; }

    public AdministrationServiceDbContext(DbContextOptions<AdministrationServiceDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ConfigurePermissionManagement();
        modelBuilder.ConfigureSettingManagement();
        modelBuilder.ConfigureFeatureManagement();
        modelBuilder.ConfigureAuditLogging();
        modelBuilder.ConfigureLanguageManagement();
        modelBuilder.ConfigureTextTemplateManagement();
        modelBuilder.ConfigureBlobStoring();
    }
}
