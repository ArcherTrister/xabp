// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Diagnostics.HealthChecks;

using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;

using IIdentityRoleRepository = X.Abp.Identity.IIdentityRoleRepository;

namespace AbpVnext.Pro.HealthChecks;

public class ProDatabaseCheck : IHealthCheck, ITransientDependency
{
    private readonly IIdentityRoleRepository identityRoleRepository;

    public ProDatabaseCheck(IIdentityRoleRepository identityRoleRepository)
    {
        this.identityRoleRepository = identityRoleRepository ?? throw new ArgumentNullException(nameof(identityRoleRepository));
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await identityRoleRepository.GetListAsync(sorting: nameof(IdentityRole.Id), maxResultCount: 1, cancellationToken: cancellationToken);

            return HealthCheckResult.Healthy("Could connect to database and get record.");
        }
        catch (Exception e)
        {
            return HealthCheckResult.Unhealthy("Error when trying to get database record.", e);
        }
    }
}
