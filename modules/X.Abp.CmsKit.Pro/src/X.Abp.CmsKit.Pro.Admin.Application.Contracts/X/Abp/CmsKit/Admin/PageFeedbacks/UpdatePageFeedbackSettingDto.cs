// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

using Volo.Abp.Validation;
using Volo.CmsKit.Localization;

using X.Abp.CmsKit.PageFeedbacks;

namespace X.Abp.CmsKit.Admin.PageFeedbacks;

[Serializable]
public class UpdatePageFeedbackSettingDto : IValidatableObject
{
    public Guid Id { get; set; }

    [DynamicMaxLength(typeof(PageFeedbackConst), nameof(PageFeedbackConst.MaxEntityTypeLength))]
    public string EntityType { get; set; }

    [DynamicMaxLength(typeof(PageFeedbackConst), nameof(PageFeedbackConst.MaxEmailAddressesLength))]
    public string EmailAddresses { get; set; }

    public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        IStringLocalizer<CmsKitResource> localizer = validationContext.GetRequiredService<IStringLocalizer<CmsKitResource>>();
        if (!string.IsNullOrWhiteSpace(EmailAddresses))
        {
            string[] strArray = EmailAddresses.Split(",");
            for (int index = 0; index < strArray.Length; ++index)
            {
                string emailAddress = strArray[index];
                if (!ValidationHelper.IsValidEmailAddress(emailAddress.Trim()))
                {
                    yield return new ValidationResult((string)localizer["InvalidEmailAddress", emailAddress], new string[] { "EmailAddresses" });
                }
            }
        }
    }
}
