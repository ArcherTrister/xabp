// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.ComponentModel.DataAnnotations;

using X.Abp.Identity.Settings;

namespace X.Abp.Identity;
public class IdentitySessionSettingsDto
{
    [Display(Name = "DisplayName:Abp.Identity.PreventConcurrentLogin")]
    public IdentityProPreventConcurrentLoginBehaviour PreventConcurrentLogin { get; set; }
}
