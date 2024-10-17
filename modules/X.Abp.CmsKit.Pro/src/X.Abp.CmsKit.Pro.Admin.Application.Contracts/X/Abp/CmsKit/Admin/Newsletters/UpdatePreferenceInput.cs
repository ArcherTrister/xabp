// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Volo.Abp.Validation;

using X.Abp.CmsKit.Newsletters;

namespace X.Abp.CmsKit.Admin.Newsletters;

public class UpdatePreferenceInput
{
    [Required]
    [DataType(DataType.EmailAddress)]
    [DynamicStringLength(typeof(NewsletterRecordConst), nameof(NewsletterRecordConst.MaxEmailAddressLength), null)]
    public string EmailAddress { get; set; }

    [Required]
    public List<PreferenceDetailsDto> PreferenceDetails { get; set; }

    [Required]
    [DynamicStringLength(typeof(NewsletterPreferenceConst), nameof(NewsletterPreferenceConst.MaxSourceLength), null)]
    public string Source { get; set; }

    [Required]
    [DynamicStringLength(typeof(NewsletterPreferenceConst), nameof(NewsletterPreferenceConst.MaxSourceUrlLength), null)]
    public string SourceUrl { get; set; }
}
