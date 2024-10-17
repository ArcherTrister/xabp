// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.ComponentModel.DataAnnotations;

using Volo.Abp.Validation;

using X.Abp.CmsKit.Faqs;

namespace X.Abp.CmsKit.Admin.Faqs;

[Serializable]
public class CreateFaqQuestionDto
{
    [Required]
    public Guid SectionId { get; set; }

    [Required]
    [DynamicMaxLength(typeof(FaqQuestionConst), nameof(FaqQuestionConst.MaxTitleLength))]
    public string Title { get; set; }

    [Required]
    [DynamicMaxLength(typeof(FaqQuestionConst), nameof(FaqQuestionConst.MaxTextLength))]
    public string Text { get; set; }

    public int Order { get; set; }
}
