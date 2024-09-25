// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Volo.Abp.Domain.Repositories;

namespace X.Abp.TextTemplateManagement.TextTemplates;
public interface ITextTemplateDefinitionContentRecordRepository : IBasicRepository<TextTemplateDefinitionContentRecord, Guid>
{
    Task<List<TextTemplateDefinitionContentRecord>> GetListByDefinitionIdAsync(
      Guid definitionId,
      CancellationToken cancellationToken = default);

    Task DeleteByDefinitionIdAsync(Guid definitionId, CancellationToken cancellationToken = default);

    Task DeleteByDefinitionIdAsync(Guid[] definitionIds, CancellationToken cancellationToken = default);

    Task<TextTemplateDefinitionContentRecord> FindByDefinitionNameAsync(
      string definitionName,
      string definitionCultureName = null,
      CancellationToken cancellationToken = default);
}
