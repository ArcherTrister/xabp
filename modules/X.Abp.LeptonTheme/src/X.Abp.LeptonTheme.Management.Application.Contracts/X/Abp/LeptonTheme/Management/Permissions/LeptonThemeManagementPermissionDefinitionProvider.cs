// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Features;
using Volo.Abp.Localization;
using X.Abp.LeptonTheme.Management.Features;
using X.Abp.LeptonTheme.Management.Localization;

namespace X.Abp.LeptonTheme.Management.Permissions
{
    public class LeptonThemeManagementPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var myGroup = context.AddGroup(LeptonThemeManagementPermissions.GroupName, L("Permission:LeptonThemeManagement"));

            myGroup.AddPermission(LeptonThemeManagementPermissions.Settings.SettingsGroup, L("Permission:LeptonThemeSettings")).
                RequireFeatures(LeptonThemeManagementFeatures.Enable);
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<LeptonThemeManagementResource>(name);
        }
    }
}
