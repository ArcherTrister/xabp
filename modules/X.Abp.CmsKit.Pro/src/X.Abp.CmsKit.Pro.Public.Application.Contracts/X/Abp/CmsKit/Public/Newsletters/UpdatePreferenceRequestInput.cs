// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Volo.Abp.Validation;

using X.Abp.CmsKit.Newsletters;

namespace X.Abp.CmsKit.Public.Newsletters;

public class UpdatePreferenceRequestInput
{
    [Required]
    [DynamicStringLength(typeof(NewsletterRecordConst), "MaxEmailAddressLength", null)]
    [DataType(DataType.EmailAddress)]
    public string EmailAddress { get; set; }

    [Required]
    public List<PreferenceDetailsDto> PreferenceDetails { get; set; }

    [Required]
    [DynamicStringLength(typeof(NewsletterPreferenceConst), "MaxSourceLength", null)]
    public string Source { get; set; }

    [Required]
    [DynamicStringLength(typeof(NewsletterPreferenceConst), "MaxSourceUrlLength", null)]
    public string SourceUrl { get; set; }

    [Required]
    public string SecurityCode { get; set; }
}
