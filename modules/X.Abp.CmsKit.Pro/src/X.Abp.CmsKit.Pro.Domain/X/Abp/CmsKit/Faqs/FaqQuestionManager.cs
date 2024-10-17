// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

namespace X.Abp.CmsKit.Faqs
{
    public class FaqQuestionManager : CmsKitProDomainServiceBase
    {
        protected IFaqQuestionRepository FaqQuestionRepository { get; }

        protected IFaqSectionRepository FaqSectionRepository { get; }

        public FaqQuestionManager(
          IFaqQuestionRepository faqQuestionRepository,
          IFaqSectionRepository faqSectionRepository)
        {
            FaqQuestionRepository = faqQuestionRepository;
            FaqSectionRepository = faqSectionRepository;
        }

        public virtual async Task<FaqQuestion> CreateAsync(
          Guid sectionId,
          string title,
          string text,
          int order)
        {
            if (await FaqQuestionRepository.AnyAsync(sectionId, title))
            {
                throw new FaqQuestionHasAlreadyException(title);
            }

            if (!await FaqSectionRepository.AnyAsync(sectionId))
            {
                throw new FaqQuestionSectionNotFound();
            }

            return new FaqQuestion(GuidGenerator.Create(), sectionId, title, text, order);
        }

        public virtual async Task UpdateTitle(FaqQuestion question, string title)
        {
            bool flag = question.Title != title;
            if (flag)
            {
                flag = await FaqQuestionRepository.AnyAsync(question.SectionId, title);
            }

            if (flag)
            {
                throw new FaqQuestionHasAlreadyException(title);
            }

            question.SetTitle(title);
        }
    }
}
