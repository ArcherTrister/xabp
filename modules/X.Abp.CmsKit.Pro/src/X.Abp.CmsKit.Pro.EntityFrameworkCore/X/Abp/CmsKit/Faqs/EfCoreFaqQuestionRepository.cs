// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

using X.Abp.CmsKit.EntityFrameworkCore;

namespace X.Abp.CmsKit.Faqs
{
    public class EfCoreFaqQuestionRepository : EfCoreRepository<ICmsKitProDbContext, FaqQuestion, Guid>, IFaqQuestionRepository
    {
        public EfCoreFaqQuestionRepository(IDbContextProvider<ICmsKitProDbContext> dbContextProvider)
          : base(dbContextProvider)
        {
        }

        public virtual async Task<List<FaqQuestion>> GetListAsync(
          Guid sectionId,
          string filter = null,
          string sorting = null,
          int skipCount = 0,
          int maxResultCount = int.MaxValue,
          CancellationToken cancellationToken = default)
        {
            return await (await GetQueryableAsync())
                .Where(faqQuestion => faqQuestion.SectionId == sectionId)
                .WhereIf(!string.IsNullOrWhiteSpace(filter), faqQuestion => faqQuestion.Title.Contains(filter) || faqQuestion.Text.Contains(filter))
                .OrderBy(string.IsNullOrWhiteSpace(sorting) ? FaqQuestionConst.DefaultSorting : sorting)
                .ThenBy(string.IsNullOrWhiteSpace(sorting) ? nameof(FaqQuestion.Title) : string.Empty)
                .PageBy(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<long> GetCountAsync(
          Guid sectionId,
          string filter = null,
          CancellationToken cancellationToken = default)
        {
            return await (await GetQueryableAsync())
                .Where(faqQuestion => faqQuestion.SectionId == sectionId)
                .WhereIf(!string.IsNullOrWhiteSpace(filter), faqQuestion => faqQuestion.Title.Contains(filter) || faqQuestion.Text.Contains(filter))
                .LongCountAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<bool> AnyAsync(
          Guid sectionId,
          string title,
          CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync()).AnyAsync(faqQuestion => faqQuestion.SectionId == sectionId && faqQuestion.Title == title, GetCancellationToken(cancellationToken));
        }
    }
}
