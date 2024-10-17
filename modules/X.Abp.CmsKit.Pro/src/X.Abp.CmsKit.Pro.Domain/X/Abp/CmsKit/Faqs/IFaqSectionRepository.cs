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
    public interface IFaqSectionRepository : IBasicRepository<FaqSection, Guid>
    {
        Task<List<FaqSectionWithQuestionCount>> GetListAsync(
          string filter = null,
          string sorting = null,
          int skipCount = 0,
          int maxResultCount = int.MaxValue,
          CancellationToken cancellationToken = default);

        Task<long> GetCountAsync(string filter = null, CancellationToken cancellationToken = default);

        Task<List<FaqSectionWithQuestions>> GetListSectionWithQuestionAsync(
          string groupName = null,
          string sectionName = null,
          CancellationToken cancellationToken = default);

        Task<bool> AnyAsync(Guid id, CancellationToken cancellationToken = default);

        Task<bool> AnyAsync(string groupName, string name, CancellationToken cancellationToken = default);
    }
}
