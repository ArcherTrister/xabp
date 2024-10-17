// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace X.Abp.CmsKit.Faqs
{
    public class FaqQuestion : FullAuditedAggregateRoot<Guid>
    {
        public virtual Guid SectionId { get; internal set; }

        public virtual string Title { get; protected set; }

        public virtual string Text { get; protected set; }

        public virtual int Order { get; set; }

        protected FaqQuestion()
        {
        }

        internal FaqQuestion(Guid id, Guid sectionId, string title, string text, int order = 0)
          : base(id)
        {
            SectionId = sectionId;
            SetTitle(title);
            SetText(text);
            Order = order;
        }

        internal virtual FaqQuestion SetTitle(string title)
        {
            Title = Check.NotNullOrWhiteSpace(title, nameof(title), FaqQuestionConst.MaxTitleLength);
            return this;
        }

        public virtual FaqQuestion SetText(string text)
        {
            Text = Check.NotNullOrWhiteSpace(text, nameof(text), FaqQuestionConst.MaxTextLength);
            return this;
        }
    }
}
