// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Threading;

namespace X.Abp.LanguageManagement.EntityFrameworkCore;

public class EfCoreLanguageTextRepository : EfCoreRepository<ILanguageManagementDbContext, LanguageText, Guid>, ILanguageTextRepository
{
    public EfCoreLanguageTextRepository(IDbContextProvider<ILanguageManagementDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    [Obsolete("Use GetListAsync() method.")]
    public virtual List<LanguageText> GetList(string resourceName, string cultureName)
    {
        using (Volo.Abp.Uow.UnitOfWorkManager.DisableObsoleteDbContextCreationWarning.SetScoped(true))
        {
            return DbSet.Where(languageText => languageText.ResourceName == resourceName && languageText.CultureName == cultureName).ToList();
        }
    }

    public async Task<List<LanguageText>> GetListAsync(string resourceName, string cultureName, CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync()).Where(languageText => languageText.ResourceName == resourceName && languageText.CultureName == cultureName).ToListAsync(GetCancellationToken(cancellationToken));
    }
}
