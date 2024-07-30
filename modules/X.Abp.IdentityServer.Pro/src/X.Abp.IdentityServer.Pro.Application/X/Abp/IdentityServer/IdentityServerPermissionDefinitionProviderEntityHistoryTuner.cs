// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.Auditing;
using Volo.Abp.Authorization.Permissions;

using X.Abp.IdentityServer.Permissions;

namespace X.Abp.IdentityServer;

public class IdentityServerPermissionDefinitionProviderEntityHistoryTuner : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var requiredService = context.ServiceProvider.GetRequiredService<IAuditingHelper>();
        if (!requiredService.IsEntityHistoryEnabled(typeof(Volo.Abp.IdentityServer.Clients.Client)))
        {
            context.TryDisablePermission(AbpIdentityServerProPermissions.Client.ViewChangeHistory);
        }

        if (!requiredService.IsEntityHistoryEnabled(typeof(Volo.Abp.IdentityServer.IdentityResources.IdentityResource)))
        {
            context.TryDisablePermission(AbpIdentityServerProPermissions.IdentityResource.ViewChangeHistory);
        }

        if (!requiredService.IsEntityHistoryEnabled(typeof(Volo.Abp.IdentityServer.ApiResources.ApiResource)))
        {
            context.TryDisablePermission(AbpIdentityServerProPermissions.ApiResource.ViewChangeHistory);
        }
    }
}
