// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

using X.Abp.TextTemplateManagement.EntityFrameworkCore;

namespace X.Abp.TextTemplateManagement.TextTemplates;
public class EfCoreTextTemplateDefinitionRecordRepository :
  EfCoreRepository<ITextTemplateManagementDbContext, TextTemplateDefinitionRecord, Guid>,
  ITextTemplateDefinitionRecordRepository
{
    public EfCoreTextTemplateDefinitionRecordRepository(IDbContextProvider<ITextTemplateManagementDbContext> dbContextProvider)
      : base(dbContextProvider)
    {
    }

    public virtual async Task<TextTemplateDefinitionRecord> FindByNameAsync(
      string name,
      CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync()).OrderBy(definitionRecord => definitionRecord.Id).FirstOrDefaultAsync(definitionRecord => definitionRecord.Name == name, cancellationToken);
    }
}
