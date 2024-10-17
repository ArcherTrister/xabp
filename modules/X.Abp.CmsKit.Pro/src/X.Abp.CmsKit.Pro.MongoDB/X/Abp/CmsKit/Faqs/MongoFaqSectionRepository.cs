// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;

using X.Abp.CmsKit.Pro.MongoDB;

namespace X.Abp.CmsKit.Faqs
{
    public class MongoFaqSectionRepository : MongoDbRepository<ICmsKitProMongoDbContext, FaqSection, Guid>, IFaqSectionRepository
    {
        public MongoFaqSectionRepository(IMongoDbContextProvider<ICmsKitProMongoDbContext> dbContextProvider)
          : base(dbContextProvider)
        {
        }

        public async Task<List<FaqSectionWithQuestionCount>> GetListAsync(
          string filter = null,
          string sorting = null,
          int skipCount = 0,
          int maxResultCount = int.MaxValue,
          CancellationToken cancellationToken = default)
        {
            IMongoQueryable<FaqSection> sectionQueryable = await GetMongoQueryableAsync(cancellationToken, null);
            IMongoQueryable<FaqQuestion> questionQueryable = await GetMongoQueryableAsync<FaqQuestion>(cancellationToken);

            var sections = await sectionQueryable
                .WhereIf(!string.IsNullOrWhiteSpace(filter), faqSection => faqSection.Name.Contains(filter))
                .OrderBy(string.IsNullOrWhiteSpace(sorting) ? FaqSectionConst.DefaultSorting : sorting)
                .ThenBy(string.IsNullOrWhiteSpace(sorting) ? nameof(FaqSection.Name) : string.Empty)
                .As<IMongoQueryable<FaqSection>>()
                .Select(p => new FaqSectionWithQuestionCount
                {
                    Id = p.Id,
                    CreationTime = p.CreationTime,
                    GroupName = p.GroupName,
                    Name = p.Name,
                    Order = p.Order
                }).ToListAsync(GetCancellationToken(cancellationToken));

            List<Guid> sectionIds = sections.Select(s => s.Id).ToList();
            var source = await questionQueryable
                .Where(faqQuestion => sectionIds.Contains(faqQuestion.SectionId))
                .GroupBy(faqQuestion => faqQuestion.SectionId)
                .Select(grouping => new
                {
                    SectionId = grouping.Key,
                    Count = grouping.Count()
                })
                .ToListAsync(GetCancellationToken(cancellationToken));

            foreach (var section in sections)
            {
                var data = source.FirstOrDefault(q => q.SectionId == section.Id);
                section.QuestionCount = data != null ? data.Count : 0;
            }

            return sections;
        }

        public async Task<long> GetCountAsync(string filter = null, CancellationToken cancellationToken = default)
        {
            return await (await GetMongoQueryableAsync(cancellationToken, null))
                .WhereIf(!filter.IsNullOrWhiteSpace(), faqSection => faqSection.Name.Contains(filter))
                .As<IMongoQueryable<FaqSection>>()
                .LongCountAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<List<FaqSectionWithQuestions>> GetListSectionWithQuestionAsync(
          string groupName,
          string sectionName,
          CancellationToken cancellationToken)
        {
            IMongoQueryable<FaqSection> sectionQueryable = await GetMongoQueryableAsync(cancellationToken, null);
            IMongoQueryable<FaqQuestion> questionQueryable = await GetMongoQueryableAsync<FaqQuestion>(cancellationToken);

            List<FaqSectionWithQuestions> sectionWithQuestions = await sectionQueryable
                .WhereIf(!string.IsNullOrWhiteSpace(groupName), faqSection => faqSection.GroupName.Contains(groupName))
                .WhereIf(!string.IsNullOrWhiteSpace(sectionName), faqSection => faqSection.Name.Contains(sectionName))
                .OrderBy(faqSection => faqSection.Order)
                .As<IMongoQueryable<FaqSection>>()
                .Select(p => new FaqSectionWithQuestions
                {
                    Section = p
                })
                .ToListAsync(GetCancellationToken(cancellationToken));

            List<Guid> sectionIds = sectionWithQuestions.Select(s => s.Section.Id).ToList();
            var source = await questionQueryable.Where(faqQuestion => sectionIds.Contains(faqQuestion.SectionId))
                .OrderBy(faqQuestion => faqQuestion.Order)
                .GroupBy(faqQuestion => faqQuestion.SectionId)
                .Select(grouping => new
                {
                    SectionId = grouping.Key,
                    Questions = grouping.ToList()
                })
                .ToListAsync(GetCancellationToken(cancellationToken));

            foreach (FaqSectionWithQuestions sectionWithQuestion in sectionWithQuestions)
            {
                var data = source.FirstOrDefault(q => q.SectionId == sectionWithQuestion.Section.Id);
                if (data != null && data.Questions != null)
                {
                    sectionWithQuestion.Questions = data.Questions;
                }
                else
                {
                    sectionWithQuestion.Questions = new List<FaqQuestion>();
                }
            }

            return sectionWithQuestions;
        }

        public virtual async Task<bool> AnyAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await (await GetMongoQueryableAsync(cancellationToken, null)).AnyAsync(faqSection => faqSection.Id == id, GetCancellationToken(cancellationToken));
        }

        public async Task<bool> AnyAsync(
          string groupName,
          string name,
          CancellationToken cancellationToken = default)
        {
            return await (await GetMongoQueryableAsync(cancellationToken, null)).AnyAsync(faqSection => faqSection.GroupName == groupName && faqSection.Name == name, GetCancellationToken(cancellationToken));
        }
    }
}
