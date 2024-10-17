// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

namespace X.Abp.CmsKit.Admin.Faqs;

[Serializable]
public class FaqSectionWithQuestionCountDto
{
    public virtual Guid Id { get; set; }

    public virtual string GroupName { get; set; }

    public virtual string Name { get; set; }

    public virtual int Order { get; set; }

    public int QuestionCount { get; set; }

    public virtual DateTime CreationTime { get; set; }
}
