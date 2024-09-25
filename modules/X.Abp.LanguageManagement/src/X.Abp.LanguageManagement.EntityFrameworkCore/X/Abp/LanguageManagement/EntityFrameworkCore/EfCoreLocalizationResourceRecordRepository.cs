// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Threading;

using X.Abp.LanguageManagement.External;

namespace X.Abp.LanguageManagement.EntityFrameworkCore
{
  public class EfCoreLocalizationResourceRecordRepository
      : EfCoreRepository<ILanguageManagementDbContext, LocalizationResourceRecord, Guid>,
          ILocalizationResourceRecordRepository
  {
    public EfCoreLocalizationResourceRecordRepository(
        IDbContextProvider<ILanguageManagementDbContext> dbContextProvider
    )
        : base(dbContextProvider) { }

    [Obsolete("Use FindAsync() method.")]
    public LocalizationResourceRecord Find(string name)
    {
      using (Volo.Abp.Uow.UnitOfWorkManager.DisableObsoleteDbContextCreationWarning.SetScoped(true))
      {
        return DbSet.FirstOrDefault(localizationResourceRecord => localizationResourceRecord.Name == name);
      }
    }

    public virtual async Task<LocalizationResourceRecord> FindAsync(
        string name,
        CancellationToken cancellationToken = default
    )
    {
      return await FindAsync(
          localizationResourceRecord => localizationResourceRecord.Name == name,
          true,
          cancellationToken
      );
    }
  }
}
