// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

using X.Abp.Account.Localization;

namespace X.Abp.Account;

public class AccountPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var identityGroup = context.AddGroup(AccountPermissions.GroupName, L("Permission:Account"));

        identityGroup.AddPermission(AccountPermissions.SettingManagement, L("Permission:SettingManagement"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<AccountResource>(name);
    }
}
