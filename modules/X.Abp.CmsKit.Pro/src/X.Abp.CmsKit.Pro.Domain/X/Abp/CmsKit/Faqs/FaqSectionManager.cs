// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

namespace X.Abp.CmsKit.Faqs
{
    public class FaqSectionManager : CmsKitProDomainServiceBase
    {
        protected IFaqSectionRepository FaqSectionRepository { get; }

        protected FaqOptions FaqOptions { get; }

        public FaqSectionManager(
          IFaqSectionRepository faqSectionRepository,
          IOptions<FaqOptions> options)
        {
            FaqSectionRepository = faqSectionRepository;
            FaqOptions = options.Value;
        }

        public virtual async Task<FaqSection> CreateAsync([NotNull] string groupName, [NotNull] string name)
        {
            if (!FaqOptions.HasGroup(groupName))
            {
                throw new FaqSectionInvalidGroupNameException(groupName);
            }

            if (await FaqSectionRepository.AnyAsync(groupName, name))
            {
                throw new FaqSectionHasAlreadyException(groupName, name);
            }

            return new FaqSection(GuidGenerator.Create(), groupName, name);
        }

        public virtual async Task UpdateAsync(FaqSection section, [NotNull] string groupName, [NotNull] string name)
        {
            if (!FaqOptions.HasGroup(groupName))
            {
                throw new FaqSectionInvalidGroupNameException(groupName);
            }

            bool flag = section.GroupName != groupName || section.Name != name;
            if (flag)
            {
                flag = await FaqSectionRepository.AnyAsync(groupName, name);
            }

            if (flag)
            {
                throw new FaqSectionHasAlreadyException(groupName, name);
            }

            section.SetGroupName(groupName).SetName(name);
        }
    }
}
