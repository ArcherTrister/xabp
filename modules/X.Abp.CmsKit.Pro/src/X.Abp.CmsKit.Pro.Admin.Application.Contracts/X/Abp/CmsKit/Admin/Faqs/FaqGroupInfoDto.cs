// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.ComponentModel.DataAnnotations;

using Volo.Abp.Validation;

using X.Abp.CmsKit.Faqs;

namespace X.Abp.CmsKit.Admin.Faqs;

[Serializable]
public class FaqGroupInfoDto
{
    [Required]
    [DynamicMaxLength(typeof(FaqSectionConst), nameof(FaqSectionConst.MaxGroupNameLength))]
    public string Name { get; set; }
}
