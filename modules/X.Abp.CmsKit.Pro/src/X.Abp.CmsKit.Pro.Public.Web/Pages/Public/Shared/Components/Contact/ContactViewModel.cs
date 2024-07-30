// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;

namespace X.Abp.CmsKit.Pro.Public.Web.Pages.Public.Shared.Components.Contact;

public class ContactViewModel
{
    public string ContactName { get; set; }

    [Required]
    [Display(Name = "Name")]
    [Placeholder("YourFullName")]
    public string Name { get; set; }

    [Placeholder("SubjectPlaceholder")]
    [Required]
    [Display(Name = "Subject")]
    public string Subject { get; set; }

    [Placeholder("YourEmailAddress")]
    [Display(Name = "EmailAddress")]
    [Required]
    public string EmailAddress { get; set; }

    [Display(Name = "YourMessage")]
    [Placeholder("HowMayWeHelpYou")]
    [Required]
    [TextArea(Rows = 3)]
    public string Message { get; set; }

    [HiddenInput]
    public string CaptchaToken { get; set; }
}
