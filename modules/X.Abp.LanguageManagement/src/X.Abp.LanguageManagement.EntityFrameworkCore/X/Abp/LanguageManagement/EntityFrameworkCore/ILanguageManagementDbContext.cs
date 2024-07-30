// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.EntityFrameworkCore;

using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

using X.Abp.LanguageManagement.External;

namespace X.Abp.LanguageManagement.EntityFrameworkCore;

[ConnectionStringName(LanguageManagementDbProperties.ConnectionStringName)]
public interface ILanguageManagementDbContext : IEfCoreDbContext
{
    DbSet<Language> Languages { get; }

    DbSet<LanguageText> LanguageTexts { get; }

    DbSet<LocalizationResourceRecord> LocalizationResources { get; }

    DbSet<LocalizationTextRecord> LocalizationTexts { get; }
}
