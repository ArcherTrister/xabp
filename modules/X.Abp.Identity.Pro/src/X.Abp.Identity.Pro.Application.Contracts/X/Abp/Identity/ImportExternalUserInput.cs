// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.ComponentModel.DataAnnotations;

using Volo.Abp.Auditing;

namespace X.Abp.Identity;

public class ImportExternalUserInput
{
    [Required]
    public string Provider { get; set; }

    [Required]
    public string UserNameOrEmailAddress { get; set; }

    [DisableAuditing]
    public string Password { get; set; }
}
