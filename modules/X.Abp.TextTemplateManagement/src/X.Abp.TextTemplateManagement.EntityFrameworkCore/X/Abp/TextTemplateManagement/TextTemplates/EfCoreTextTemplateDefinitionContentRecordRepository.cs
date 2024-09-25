// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

using X.Abp.TextTemplateManagement.EntityFrameworkCore;

namespace X.Abp.TextTemplateManagement.TextTemplates;
public class EfCoreTextTemplateDefinitionContentRecordRepository :
  EfCoreRepository<ITextTemplateManagementDbContext, TextTemplateDefinitionContentRecord, Guid>,
  ITextTemplateDefinitionContentRecordRepository
{
    public EfCoreTextTemplateDefinitionContentRecordRepository(
      IDbContextProvider<ITextTemplateManagementDbContext> dbContextProvider)
      : base(dbContextProvider)
    {
    }

    public virtual async Task<List<TextTemplateDefinitionContentRecord>> GetListByDefinitionIdAsync(
      Guid definitionId,
      CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync()).Where(definitionContentRecord => definitionContentRecord.DefinitionId == definitionId).ToListAsync(GetCancellationToken(cancellationToken));
    }

    public virtual async Task DeleteByDefinitionIdAsync(
      Guid definitionId,
      CancellationToken cancellationToken = default)
    {
        await DeleteAsync(definitionContentRecord => definitionContentRecord.DefinitionId == definitionId, false, cancellationToken);
    }

    public virtual async Task DeleteByDefinitionIdAsync(
      Guid[] definitionIds,
      CancellationToken cancellationToken = default)
    {
        await DeleteAsync(definitionContentRecord => definitionIds.Contains(definitionContentRecord.DefinitionId), false, cancellationToken);
    }

    public virtual async Task<TextTemplateDefinitionContentRecord> FindByDefinitionNameAsync(
      string definitionName,
      string definitionCultureName = null,
      CancellationToken cancellationToken = default)
    {
        TextTemplateDefinitionRecord textTemplateDefinitionRecord = await (await GetDbContextAsync()).Set<TextTemplateDefinitionRecord>().FirstAsync(definitionRecord => definitionRecord.Name == definitionName, cancellationToken);
        if (definitionCultureName != null)
        {
            List<string> templateFileNames = new List<string>()
            {
              definitionCultureName + ".tpl",
              definitionCultureName + ".cshtml"
            };
            return await (await GetDbSetAsync()).FirstOrDefaultAsync(definitionContentRecord => definitionContentRecord.DefinitionId == textTemplateDefinitionRecord.Id && templateFileNames.Contains(definitionContentRecord.FileName), cancellationToken);
        }

        return await (await GetDbSetAsync()).Where(definitionContentRecord => definitionContentRecord.DefinitionId == textTemplateDefinitionRecord.Id).FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
    }
}
