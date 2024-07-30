// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Authorization.Permissions;
using Volo.Abp.IdentityServer.Localization;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace X.Abp.IdentityServer.Permissions;

public class AbpIdentityServerProPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var identityServerGroup = context.AddGroup(AbpIdentityServerProPermissions.GroupName, L("Permission:IdentityServer"));

        var apiScopePermissions = identityServerGroup.AddPermission(AbpIdentityServerProPermissions.ApiScope.Default, L("Permission:ApiScope"), MultiTenancySides.Host, true);
        apiScopePermissions.AddChild(AbpIdentityServerProPermissions.ApiScope.Update, L("Permission:Edit"), MultiTenancySides.Host, true);
        apiScopePermissions.AddChild(AbpIdentityServerProPermissions.ApiScope.Delete, L("Permission:Delete"), MultiTenancySides.Host, true);
        apiScopePermissions.AddChild(AbpIdentityServerProPermissions.ApiScope.Create, L("Permission:Create"), MultiTenancySides.Host, true);
        apiScopePermissions.AddChild(AbpIdentityServerProPermissions.ApiScope.ViewChangeHistory, L("Permission:ViewChangeHistory"), MultiTenancySides.Host, true);

        var identityResourcePermissions = identityServerGroup.AddPermission(AbpIdentityServerProPermissions.IdentityResource.Default, L("Permission:IdentityResources"), MultiTenancySides.Host, true);
        identityResourcePermissions.AddChild(AbpIdentityServerProPermissions.IdentityResource.Update, L("Permission:Edit"), MultiTenancySides.Host, true);
        identityResourcePermissions.AddChild(AbpIdentityServerProPermissions.IdentityResource.Delete, L("Permission:Delete"), MultiTenancySides.Host, true);
        identityResourcePermissions.AddChild(AbpIdentityServerProPermissions.IdentityResource.Create, L("Permission:Create"), MultiTenancySides.Host, true);
        identityResourcePermissions.AddChild(AbpIdentityServerProPermissions.IdentityResource.ViewChangeHistory, L("Permission:ViewChangeHistory"), MultiTenancySides.Host, true);

        var apiResourcePermissions = identityServerGroup.AddPermission(AbpIdentityServerProPermissions.ApiResource.Default, L("Permission:ApiResources"), MultiTenancySides.Host, true);
        apiResourcePermissions.AddChild(AbpIdentityServerProPermissions.ApiResource.Update, L("Permission:Edit"), MultiTenancySides.Host, true);
        apiResourcePermissions.AddChild(AbpIdentityServerProPermissions.ApiResource.Delete, L("Permission:Delete"), MultiTenancySides.Host, true);
        apiResourcePermissions.AddChild(AbpIdentityServerProPermissions.ApiResource.Create, L("Permission:Create"), MultiTenancySides.Host, true);
        apiResourcePermissions.AddChild(AbpIdentityServerProPermissions.ApiResource.ViewChangeHistory, L("Permission:ViewChangeHistory"), MultiTenancySides.Host, true);

        var clientPermissions = identityServerGroup.AddPermission(AbpIdentityServerProPermissions.Client.Default, L("Permission:Clients"), MultiTenancySides.Host, true);
        clientPermissions.AddChild(AbpIdentityServerProPermissions.Client.Update, L("Permission:Edit"), MultiTenancySides.Host, true);
        clientPermissions.AddChild(AbpIdentityServerProPermissions.Client.Delete, L("Permission:Delete"), MultiTenancySides.Host, true);
        clientPermissions.AddChild(AbpIdentityServerProPermissions.Client.Create, L("Permission:Create"), MultiTenancySides.Host, true);
        clientPermissions.AddChild(AbpIdentityServerProPermissions.Client.ManagePermissions, L("Permission:ManagePermissions"), MultiTenancySides.Host, true);
        clientPermissions.AddChild(AbpIdentityServerProPermissions.Client.ViewChangeHistory, L("Permission:ViewChangeHistory"), MultiTenancySides.Host, true);
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<AbpIdentityServerResource>(name);
    }
}
