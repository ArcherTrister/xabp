using MyCompanyName.MyProjectName.AdministrationService.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace MyCompanyName.MyProjectName.AdministrationService.Permissions;

public class AdministrationServicePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(AdministrationServicePermissions.GroupName);

        myGroup.AddPermission(AdministrationServicePermissions.Dashboard.Host, L("Permission:Dashboard"), MultiTenancySides.Host);
        myGroup.AddPermission(AdministrationServicePermissions.Dashboard.Tenant, L("Permission:Dashboard"), MultiTenancySides.Tenant);
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<AdministrationServiceResource>(name);
    }
}
