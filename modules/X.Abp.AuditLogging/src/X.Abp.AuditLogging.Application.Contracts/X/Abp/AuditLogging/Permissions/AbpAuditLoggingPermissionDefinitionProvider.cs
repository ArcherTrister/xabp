// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.AuditLogging.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Features;
using Volo.Abp.Localization;

using X.Abp.AuditLogging.Features;

namespace X.Abp.AuditLogging.Permissions;

public class AbpAuditLoggingPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var permissionGroup = context.AddGroup(AbpAuditLoggingPermissions.GroupName, L("Permission:AuditLogging"));

        permissionGroup.AddPermission(AbpAuditLoggingPermissions.AuditLogs.Default, L("Permission:AuditLogs"))
            .RequireFeatures(AbpAuditLoggingFeatures.Enable);
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<AuditLoggingResource>(name);
    }
}
