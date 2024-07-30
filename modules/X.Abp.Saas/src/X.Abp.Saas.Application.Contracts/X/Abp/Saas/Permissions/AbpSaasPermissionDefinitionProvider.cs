// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

using X.Abp.Saas.Localization;

namespace X.Abp.Saas.Permissions;

public class AbpSaasPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var saasHostGroup = context.AddGroup(AbpSaasPermissions.GroupName, L("Permission:Saas"));
        var tenantManagement = saasHostGroup.AddPermission(AbpSaasPermissions.Tenants.Default, L("Permission:TenantManagement"), MultiTenancySides.Host);
        tenantManagement.AddChild(AbpSaasPermissions.Tenants.Create, L("Permission:Create"), MultiTenancySides.Host);
        tenantManagement.AddChild(AbpSaasPermissions.Tenants.Update, L("Permission:Edit"), MultiTenancySides.Host);
        tenantManagement.AddChild(AbpSaasPermissions.Tenants.Delete, L("Permission:Delete"), MultiTenancySides.Host);
        tenantManagement.AddChild(AbpSaasPermissions.Tenants.ManageFeatures, L("Permission:ManageFeatures"), MultiTenancySides.Host);
        tenantManagement.AddChild(AbpSaasPermissions.Tenants.ManageConnectionStrings, L("Permission:ManageConnectionStrings"), MultiTenancySides.Host);
        tenantManagement.AddChild(AbpSaasPermissions.Tenants.SetPassword, L("Permission:SetPassword"), MultiTenancySides.Host);
        tenantManagement.AddChild(AbpSaasPermissions.Tenants.ViewChangeHistory, L("Permission:ViewChangeHistory"), MultiTenancySides.Host);
        tenantManagement.AddChild(AbpSaasPermissions.Tenants.Impersonation, L("Permission:Impersonation"), MultiTenancySides.Host);

        var editionManagement = saasHostGroup.AddPermission(AbpSaasPermissions.Editions.Default, L("Permission:EditionManagement"), MultiTenancySides.Host);
        editionManagement.AddChild(AbpSaasPermissions.Editions.Create, L("Permission:Create"), MultiTenancySides.Host);
        editionManagement.AddChild(AbpSaasPermissions.Editions.Update, L("Permission:Edit"), MultiTenancySides.Host);
        editionManagement.AddChild(AbpSaasPermissions.Editions.Delete, L("Permission:Delete"), MultiTenancySides.Host);
        editionManagement.AddChild(AbpSaasPermissions.Editions.ManageFeatures, L("Permission:ManageFeatures"), MultiTenancySides.Host);
        editionManagement.AddChild(AbpSaasPermissions.Editions.ViewChangeHistory, L("Permission:ViewChangeHistory"), MultiTenancySides.Host);
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<SaasResource>(name);
    }
}
