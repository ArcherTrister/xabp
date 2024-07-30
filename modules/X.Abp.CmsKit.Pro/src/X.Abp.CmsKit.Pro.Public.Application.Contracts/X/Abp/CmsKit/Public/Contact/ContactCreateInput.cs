// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.ComponentModel.DataAnnotations;

namespace X.Abp.CmsKit.Public.Contact;

public class ContactCreateInput
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Subject { get; set; }

    [DataType(DataType.EmailAddress)]
    [Required]
    public string Email { get; set; }

    [Required]
    public string Message { get; set; }

    public string CaptchaToken { get; set; }
}
