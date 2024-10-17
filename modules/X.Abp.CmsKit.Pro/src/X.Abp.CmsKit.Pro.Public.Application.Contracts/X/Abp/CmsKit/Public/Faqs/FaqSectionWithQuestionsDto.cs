// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;

namespace X.Abp.CmsKit.Public.Faqs;

[Serializable]
public class FaqSectionWithQuestionsDto
{
    public FaqSectionDto Section { get; set; }

    public List<FaqQuestionDto> Questions { get; set; }

    public FaqSectionWithQuestionsDto()
    {
        Questions = new List<FaqQuestionDto>();
    }
}
