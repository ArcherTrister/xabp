// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Volo.Abp.Validation;

using X.Abp.CmsKit.Newsletters;

namespace X.Abp.CmsKit.Public.Newsletters;

public class CreateNewsletterRecordInput
{
    [DynamicStringLength(typeof(NewsletterRecordConst), "MaxEmailAddressLength", null)]
    [Required]
    [DataType(DataType.EmailAddress)]
    public string EmailAddress { get; set; }

    [Required]
    [DynamicStringLength(typeof(NewsletterPreferenceConst), "MaxPreferenceLength", null)]
    public string Preference { get; set; }

    [Required]
    [DynamicStringLength(typeof(NewsletterPreferenceConst), "MaxSourceLength", null)]
    public string Source { get; set; }

    [Required]
    [DynamicStringLength(typeof(NewsletterPreferenceConst), "MaxSourceUrlLength", null)]
    public string SourceUrl { get; set; }

    public string PrivacyPolicyUrl { get; set; }

    public List<string> AdditionalPreferences { get; set; }
}
