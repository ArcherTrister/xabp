// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Volo.Abp.Domain.Repositories;

namespace X.Abp.CmsKit.Faqs
{
    public interface IFaqQuestionRepository : IBasicRepository<FaqQuestion, Guid>
    {
        Task<List<FaqQuestion>> GetListAsync(
          Guid sectionId,
          string filter = null,
          string sorting = null,
          int skipCount = 0,
          int maxResultCount = int.MaxValue,
          CancellationToken cancellationToken = default);

        Task<long> GetCountAsync(
          Guid sectionId,
          string filter = null,
          CancellationToken cancellationToken = default);

        Task<bool> AnyAsync(Guid sectionId, string title, CancellationToken cancellationToken = default);
    }
}
