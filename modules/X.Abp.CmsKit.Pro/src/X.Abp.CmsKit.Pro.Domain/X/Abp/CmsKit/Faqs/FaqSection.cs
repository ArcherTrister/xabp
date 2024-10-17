// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace X.Abp.CmsKit.Faqs
{
    public class FaqSection : FullAuditedAggregateRoot<Guid>
    {
        public virtual string GroupName { get; protected set; }

        public virtual string Name { get; protected set; }

        public virtual int Order { get; set; }

        protected FaqSection()
        {
        }

        public FaqSection(Guid id, string groupName, string name, int order = 0)
          : base(id)
        {
            SetGroupName(groupName);
            SetName(name);
            Order = order;
        }

        public virtual FaqSection SetGroupName(string groupName)
        {
            GroupName = Check.NotNullOrWhiteSpace(groupName, nameof(groupName), FaqSectionConst.MaxGroupNameLength);
            return this;
        }

        public virtual FaqSection SetName(string name)
        {
            Name = Check.NotNullOrWhiteSpace(name, nameof(name), FaqSectionConst.MaxNameLength);
            return this;
        }
    }
}
