// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using X.Abp.Identity.Features;

namespace X.Abp.Account;

public class AccountTwoFactorSettingsDto
{
    public IdentityProTwoFactorBehaviour TwoFactorBehaviour { get; set; }

    public bool IsRememberBrowserEnabled { get; set; }

    public bool UsersCanChange { get; set; }
}
