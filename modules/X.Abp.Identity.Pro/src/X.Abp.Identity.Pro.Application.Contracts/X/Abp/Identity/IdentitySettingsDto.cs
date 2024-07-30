// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.Identity;

public class IdentitySettingsDto
{
    public IdentityPasswordSettingsDto Password { get; set; }

    public IdentityLockoutSettingsDto Lockout { get; set; }

    public IdentitySignInSettingsDto SignIn { get; set; }

    public IdentityUserSettingsDto User { get; set; }

    public IdentityOrganizationUnitSettingsDto OrganizationUnit { get; set; }
}
