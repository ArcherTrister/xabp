// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Features;
using Volo.Abp.Localization;

using X.Abp.Forms.Localization;

namespace X.Abp.Forms.Permissions;

public class AbpFormsPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var formManagementGroup = context.AddGroup(AbpFormsPermissions.GroupName, L("Permission:Forms"));

        var formPermissions = formManagementGroup.AddPermission(AbpFormsPermissions.Forms.Default, L("Permission:Forms.Management"))
            .RequireFeatures(FormsFeatures.Enable);
        formPermissions.AddChild(AbpFormsPermissions.Forms.Delete, L("Permission:Forms:Form.Delete"))
            .RequireFeatures(FormsFeatures.Enable);

        var responsePermissions = formManagementGroup.AddPermission(AbpFormsPermissions.Response.Default, L("Permission:Forms.Response.Management"))
            .RequireFeatures(FormsFeatures.Enable);
        responsePermissions.AddChild(AbpFormsPermissions.Response.Delete, L("Permission:Forms:Response.Delete"))
            .RequireFeatures(FormsFeatures.Enable);
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<FormsResource>(name);
    }
}
