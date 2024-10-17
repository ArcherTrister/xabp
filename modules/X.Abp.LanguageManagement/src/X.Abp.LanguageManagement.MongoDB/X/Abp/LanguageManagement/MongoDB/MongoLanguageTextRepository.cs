// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;
using Volo.Abp.Threading;

namespace X.Abp.LanguageManagement.MongoDB
{
    public class MongoLanguageTextRepository : MongoDbRepository<ILanguageManagementMongoDbContext, LanguageText, Guid>, ILanguageTextRepository
    {
        public MongoLanguageTextRepository(
          IMongoDbContextProvider<ILanguageManagementMongoDbContext> dbContextProvider)
          : base(dbContextProvider)
        {
        }

        public virtual List<LanguageText> GetList(
          string resourceName,
          string cultureName)
        {
            using (Volo.Abp.Uow.UnitOfWorkManager.DisableObsoleteDbContextCreationWarning.SetScoped(true))
            {
                return GetMongoQueryable().Where(languageText => languageText.ResourceName == resourceName && languageText.CultureName == cultureName)
                    .ToList();
            }
        }

        public virtual async Task<List<LanguageText>> GetListAsync(
          string resourceName,
          string cultureName,
          CancellationToken cancellationToken = default)
        {
            return await (await GetMongoQueryableAsync(GetCancellationToken(cancellationToken), null))
                .Where(languageText => languageText.ResourceName == resourceName && languageText.CultureName == cultureName)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }
    }
}
