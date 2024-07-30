// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.Auditing;
using Volo.Abp.Authorization.Permissions;

using X.Abp.Saas.Editions;
using X.Abp.Saas.Permissions;
using X.Abp.Saas.Tenants;

namespace X.Abp.Saas;

public class SaasPermissionDefinitionProviderEntityHistoryTuner : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var auditingHelper = context.ServiceProvider.GetRequiredService<IAuditingHelper>();

        if (!auditingHelper.IsEntityHistoryEnabled(typeof(Tenant)))
        {
            context.TryDisablePermission(AbpSaasPermissions.Tenants.ViewChangeHistory);
        }

        if (!auditingHelper.IsEntityHistoryEnabled(typeof(Edition)))
        {
            context.TryDisablePermission(AbpSaasPermissions.Editions.ViewChangeHistory);
        }
    }
}
