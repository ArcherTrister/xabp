// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading;
using System.Threading.Tasks;

using Volo.Abp.Domain.Repositories;

namespace X.Abp.TextTemplateManagement.TextTemplates;

public interface ITextTemplateContentRepository : IBasicRepository<TextTemplateContent, Guid>
{
    Task<TextTemplateContent> GetAsync(
      string name,
      string cultureName = null,
      CancellationToken cancellationToken = default);

    Task<TextTemplateContent> FindAsync(
      string name,
      string cultureName = null,
      CancellationToken cancellationToken = default);
}
