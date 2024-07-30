using MyCompanyName.MyProjectName.SaasService.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace MyCompanyName.MyProjectName.SaasService.Permissions;

public class SaasServicePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(SaasServicePermissions.GroupName, L("Permission:SaasService"));

        //Define your own permissions here. Example:
        //myGroup.AddPermission(MyProjectNamePermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<SaasServiceResource>(name);
    }
}
