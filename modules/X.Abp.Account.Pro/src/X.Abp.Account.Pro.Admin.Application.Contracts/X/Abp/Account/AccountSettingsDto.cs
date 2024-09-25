// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.Account;

public class AccountSettingsDto
{
    public bool IsSelfRegistrationEnabled { get; set; }

    public bool EnableLocalLogin { get; set; }

    public bool PreventEmailEnumeration { get; set; }
}
