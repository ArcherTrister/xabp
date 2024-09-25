// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Identity.Localization;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace X.Abp.Identity.Permissions;

public class AbpIdentityProPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var identityGroup = context.AddGroup(AbpIdentityProPermissions.GroupName, L("Permission:IdentityManagement"));

        var rolesPermission = identityGroup.AddPermission(AbpIdentityProPermissions.Roles.Default, L("Permission:RoleManagement"));
        rolesPermission.AddChild(AbpIdentityProPermissions.Roles.Create, L("Permission:Create"));
        rolesPermission.AddChild(AbpIdentityProPermissions.Roles.Update, L("Permission:Edit"));
        rolesPermission.AddChild(AbpIdentityProPermissions.Roles.Delete, L("Permission:Delete"));
        rolesPermission.AddChild(AbpIdentityProPermissions.Roles.ManagePermissions, L("Permission:ChangePermissions"));
        rolesPermission.AddChild(AbpIdentityProPermissions.Roles.ViewChangeHistory, L("Permission:ViewChangeHistory"));

        var usersPermission = identityGroup.AddPermission(AbpIdentityProPermissions.Users.Default, L("Permission:UserManagement"));
        usersPermission.AddChild(AbpIdentityProPermissions.Users.Create, L("Permission:Create"));
        usersPermission.AddChild(AbpIdentityProPermissions.Users.Update, L("Permission:Edit"));
        usersPermission.AddChild(AbpIdentityProPermissions.Users.Delete, L("Permission:Delete"));
        usersPermission.AddChild(AbpIdentityProPermissions.Users.ManagePermissions, L("Permission:ChangePermissions"));
        usersPermission.AddChild(AbpIdentityProPermissions.Users.ViewChangeHistory, L("Permission:ViewChangeHistory"));
        usersPermission.AddChild(AbpIdentityProPermissions.Users.Impersonation, L("Permission:Impersonation"));
        usersPermission.AddChild(AbpIdentityProPermissions.Users.Import, L("Permission:Import"));
        usersPermission.AddChild(AbpIdentityProPermissions.Users.Export, L("Permission:Export"));
        usersPermission.AddChild(AbpIdentityProPermissions.Users.ViewDetails, L("Permission:ViewDetails"));
        usersPermission.AddChild(AbpIdentityProPermissions.Users.ManageRoles, L("Permission:ManageRoles"));
        usersPermission.AddChild(AbpIdentityProPermissions.Users.ManageOU, L("Permission:ManageOU"));

        var organizationUnitsPermission = identityGroup.AddPermission(AbpIdentityProPermissions.OrganizationUnits.Default, L("Permission:OrganizationUnitManagement"));
        organizationUnitsPermission.AddChild(AbpIdentityProPermissions.OrganizationUnits.ManageOU, L("Permission:ManageOU"));
        organizationUnitsPermission.AddChild(AbpIdentityProPermissions.OrganizationUnits.ManageRoles, L("Permission:ManageRoles"));
        organizationUnitsPermission.AddChild(AbpIdentityProPermissions.OrganizationUnits.ManageUsers, L("Permission:ManageUsers"));

        var claimTypesPermission = identityGroup.AddPermission(AbpIdentityProPermissions.ClaimTypes.Default, L("Permission:ClaimManagement"), multiTenancySide: MultiTenancySides.Host);
        claimTypesPermission.AddChild(AbpIdentityProPermissions.ClaimTypes.Create, L("Permission:Create"), multiTenancySide: MultiTenancySides.Host);
        claimTypesPermission.AddChild(AbpIdentityProPermissions.ClaimTypes.Update, L("Permission:Edit"), multiTenancySide: MultiTenancySides.Host);
        claimTypesPermission.AddChild(AbpIdentityProPermissions.ClaimTypes.Delete, L("Permission:Delete"), multiTenancySide: MultiTenancySides.Host);

        identityGroup.AddPermission(AbpIdentityProPermissions.SettingManagement, L("Permission:SettingManagement"));

        identityGroup
            .AddPermission(AbpIdentityProPermissions.UserLookup.Default, L("Permission:UserLookup"))
            .WithProviders(ClientPermissionValueProvider.ProviderName);
        identityGroup.AddPermission(AbpIdentityProPermissions.SecurityLogs.Default, L("Permission:SecurityLogs"));
        identityGroup.AddPermission(AbpIdentityProPermissions.Sessions.Default, L("Permission:Sessions"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<IdentityResource>(name);
    }
}
