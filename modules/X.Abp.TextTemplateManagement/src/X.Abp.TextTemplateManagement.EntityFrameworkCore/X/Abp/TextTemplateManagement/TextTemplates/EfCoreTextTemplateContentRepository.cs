// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading;
using System.Threading.Tasks;

using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

using X.Abp.TextTemplateManagement.EntityFrameworkCore;

namespace X.Abp.TextTemplateManagement.TextTemplates;

public class EfCoreTextTemplateContentRepository :
EfCoreRepository<ITextTemplateManagementDbContext, TextTemplateContent, Guid>,
ITextTemplateContentRepository
{
    public EfCoreTextTemplateContentRepository(
      IDbContextProvider<ITextTemplateManagementDbContext> dbContextProvider)
      : base(dbContextProvider)
    {
    }

    public virtual async Task<TextTemplateContent> GetAsync(
      string name,
      string cultureName = null,
      CancellationToken cancellationToken = default)
    {
        return await GetAsync(textTemplateContent => textTemplateContent.Name == name && textTemplateContent.CultureName == cultureName, true, GetCancellationToken(cancellationToken));
    }

    public virtual async Task<TextTemplateContent> FindAsync(
      string name,
      string cultureName = null,
      CancellationToken cancellationToken = default)
    {
        return await FindAsync(textTemplateContent => textTemplateContent.Name == name && textTemplateContent.CultureName == cultureName, true, GetCancellationToken(cancellationToken));
    }
}
