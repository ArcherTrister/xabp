// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp;

namespace X.Abp.CmsKit.Faqs
{
    public class FaqGroupInfo
    {
        public virtual string Name { get; set; }

        public FaqGroupInfo(string name)
        {
            Name = Check.NotNullOrWhiteSpace(name, nameof(name));
        }
    }
}
