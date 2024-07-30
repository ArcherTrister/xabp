// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Features;
using Volo.Abp.Localization;

using X.Abp.VersionManagement.Localization;

namespace X.Abp.VersionManagement.Permissions;

public class AbpVersionManagementPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var versionManagementGroup = context.AddGroup(AbpVersionManagementPermissions.GroupName, L("Permission:VersionManagement"));

        var appEditionsPermission = versionManagementGroup.AddPermission(AbpVersionManagementPermissions.AppEditions.Default, L("Permission:AppEditionManagement"))
            .RequireFeatures(VersionManagementFeatures.Enable);
        appEditionsPermission.AddChild(AbpVersionManagementPermissions.AppEditions.Create, L("Permission:Create"))
            .RequireFeatures(VersionManagementFeatures.Enable);
        appEditionsPermission.AddChild(AbpVersionManagementPermissions.AppEditions.Edit, L("Permission:Edit"))
            .RequireFeatures(VersionManagementFeatures.Enable);
        appEditionsPermission.AddChild(AbpVersionManagementPermissions.AppEditions.Delete, L("Permission:Delete"))
            .RequireFeatures(VersionManagementFeatures.Enable);
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<VersionManagementResource>(name);
    }
}
