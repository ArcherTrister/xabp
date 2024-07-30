// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.EntityFrameworkCore;

using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

using X.Abp.LanguageManagement.External;

namespace X.Abp.LanguageManagement.EntityFrameworkCore;

[ConnectionStringName(LanguageManagementDbProperties.ConnectionStringName)]
public class LanguageManagementDbContext : AbpDbContext<LanguageManagementDbContext>, ILanguageManagementDbContext
{
    public DbSet<Language> Languages { get; set; }

    public DbSet<LanguageText> LanguageTexts { get; set; }

    public DbSet<LocalizationResourceRecord> LocalizationResources { get; set; }

    public DbSet<LocalizationTextRecord> LocalizationTexts { get; set; }

    public LanguageManagementDbContext(DbContextOptions<LanguageManagementDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ConfigureLanguageManagement();
    }
}
