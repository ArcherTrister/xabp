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
    public class EfCoreFaqSectionRepository : EfCoreRepository<ICmsKitProDbContext, FaqSection, Guid>, IFaqSectionRepository
    {
        public EfCoreFaqSectionRepository(IDbContextProvider<ICmsKitProDbContext> dbContextProvider)
          : base(dbContextProvider)
        {
        }

        public virtual async Task<List<FaqSectionWithQuestionCount>> GetListAsync(
          string filter = null,
          string sorting = null,
          int skipCount = 0,
          int maxResultCount = int.MaxValue,
          CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return await (await GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(filter), faqQuestion => faqQuestion.Name.Contains(filter))
                .GroupJoin(dbContext.Set<FaqQuestion>(), faqSection => faqSection.Id, faqQuestion => faqQuestion.SectionId, (faqSection, faqQuestions) => new
                {
                    Section = faqSection,
                    SectionQuestions = faqQuestions
                })
                .Select(p => new FaqSectionWithQuestionCount
                {
                    Id = p.Section.Id,
                    CreationTime = p.Section.CreationTime,
                    GroupName = p.Section.GroupName,
                    Name = p.Section.Name,
                    Order = p.Section.Order,
                    QuestionCount = p.SectionQuestions.Count()
                })
                .OrderBy(string.IsNullOrWhiteSpace(sorting) ? FaqSectionConst.DefaultSorting : sorting)
                .ThenBy(string.IsNullOrWhiteSpace(sorting) ? nameof(FaqSection.Name) : string.Empty)
                .PageBy(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<long> GetCountAsync(string filter = null, CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .WhereIf(!filter.IsNullOrWhiteSpace(), faqSection => faqSection.Name.Contains(filter))
                .LongCountAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<List<FaqSectionWithQuestions>> GetListSectionWithQuestionAsync(
          string groupName,
          string sectionName,
          CancellationToken cancellationToken)
        {
            var dbContext = await GetDbContextAsync();

            return await (await GetQueryableAsync())
                .Where(data => (string.IsNullOrEmpty(groupName) || data.GroupName == groupName) && (string.IsNullOrEmpty(sectionName) || data.Name == sectionName))
                .GroupJoin(dbContext.Set<FaqQuestion>(), faqSection => faqSection.Id, faqQuestion => faqQuestion.SectionId, (faqSection, faqQuestions) => new
                {
                    Section = faqSection,
                    SectionQuestions = faqQuestions
                })
                .OrderBy(data => data.Section.Order)
                .Select(p => new FaqSectionWithQuestions { Section = p.Section, Questions = p.SectionQuestions.ToList() })
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<bool> AnyAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync()).AnyAsync(faqSection => faqSection.Id == id, GetCancellationToken(cancellationToken));
        }

        public virtual async Task<bool> AnyAsync(
          string groupName,
          string name,
          CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync()).AnyAsync(faqSection => faqSection.GroupName == groupName && faqSection.Name == name, GetCancellationToken(cancellationToken));
        }
    }
}
