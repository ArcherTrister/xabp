// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Volo.Abp.Domain.Repositories;

namespace X.Abp.CmsKit.Newsletters;

public interface INewsletterRecordRepository : IBasicRepository<NewsletterRecord, Guid>
{
    Task<List<NewsletterSummaryQueryResultItem>> GetListAsync(
      string preference = null,
      string source = null,
      string emailAddress = null,
      int skipCount = 0,
      int maxResultCount = int.MaxValue,
      CancellationToken cancellationToken = default);

    Task<NewsletterRecord> FindByEmailAddressAsync(
      string emailAddress,
      bool includeDetails = true,
      CancellationToken cancellationToken = default);

    Task<int> GetCountByFilterAsync(
      string preference = null,
      string source = null,
      string emailAddress = null,
      CancellationToken cancellationToken = default);
}
