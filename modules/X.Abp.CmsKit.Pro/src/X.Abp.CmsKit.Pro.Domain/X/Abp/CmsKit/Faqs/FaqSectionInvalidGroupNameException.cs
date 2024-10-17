// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp;

namespace X.Abp.CmsKit.Faqs
{
    [Serializable]
    public class FaqSectionInvalidGroupNameException : BusinessException
    {
        public FaqSectionInvalidGroupNameException(string groupName)
          : base(CmsKitProErrorCodes.FaqSection.FaqSectionInvalidGroupName)
        {
            WithData("GroupName", groupName);
        }
    }
}
