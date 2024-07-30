// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.Auditing;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Identity;

using X.Abp.Identity.Permissions;

namespace X.Abp.Identity;

public class IdentityPermissionDefinitionProviderEntityHistoryTuner : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var auditHelper = context.ServiceProvider.GetRequiredService<IAuditingHelper>();

        if (!auditHelper.IsEntityHistoryEnabled(typeof(IdentityUser)))
        {
            context.TryDisablePermission(AbpIdentityProPermissions.Users.ViewChangeHistory);
        }

        if (!auditHelper.IsEntityHistoryEnabled(typeof(IdentityRole)))
        {
            context.TryDisablePermission(AbpIdentityProPermissions.Roles.ViewChangeHistory);
        }
    }
}
