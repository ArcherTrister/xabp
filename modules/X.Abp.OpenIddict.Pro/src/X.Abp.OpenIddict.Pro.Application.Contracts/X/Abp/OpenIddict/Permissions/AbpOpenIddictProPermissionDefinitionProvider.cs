// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;
using Volo.Abp.OpenIddict.Localization;

namespace X.Abp.OpenIddict.Permissions;

public class AbpOpenIddictProPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var permissionGroupDefinition = context.AddGroup(AbpOpenIddictProPermissions.GroupName, L("Permission:OpenIddictPro"));
        var applicationPermission = permissionGroupDefinition.AddPermission(AbpOpenIddictProPermissions.Application.Default, L("Permission:Application"), MultiTenancySides.Host, true);
        applicationPermission.AddChild(AbpOpenIddictProPermissions.Application.Update, L("Permission:Edit"), MultiTenancySides.Host, true);
        applicationPermission.AddChild(AbpOpenIddictProPermissions.Application.Delete, L("Permission:Delete"), MultiTenancySides.Host, true);
        applicationPermission.AddChild(AbpOpenIddictProPermissions.Application.Create, L("Permission:Create"), MultiTenancySides.Host, true);
        applicationPermission.AddChild(AbpOpenIddictProPermissions.Application.ManagePermissions, L("Permission:ManagePermissions"), MultiTenancySides.Host, true);
        applicationPermission.AddChild(AbpOpenIddictProPermissions.Application.ViewChangeHistory, L("Permission:ViewChangeHistory"), MultiTenancySides.Host, true);

        var scopePermission = permissionGroupDefinition.AddPermission(AbpOpenIddictProPermissions.Scope.Default, L("Permission:Scope"), MultiTenancySides.Host, true);
        scopePermission.AddChild(AbpOpenIddictProPermissions.Scope.Update, L("Permission:Edit"), MultiTenancySides.Host, true);
        scopePermission.AddChild(AbpOpenIddictProPermissions.Scope.Delete, L("Permission:Delete"), MultiTenancySides.Host, true);
        scopePermission.AddChild(AbpOpenIddictProPermissions.Scope.Create, L("Permission:Create"), MultiTenancySides.Host, true);
        scopePermission.AddChild(AbpOpenIddictProPermissions.Scope.ViewChangeHistory, L("Permission:ViewChangeHistory"), MultiTenancySides.Host, true);
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<AbpOpenIddictResource>(name);
    }
}
