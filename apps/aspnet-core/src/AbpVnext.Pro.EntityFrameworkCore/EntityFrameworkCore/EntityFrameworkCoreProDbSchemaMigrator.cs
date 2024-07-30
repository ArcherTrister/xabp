// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using AbpVnext.Pro.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace AbpVnext.Pro.EntityFrameworkCore;

public class EntityFrameworkCoreProDbSchemaMigrator
    : IProDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreProDbSchemaMigrator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the DbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope (connection string is dynamically resolved).
         */

        var dbContextType = _serviceProvider.GetRequiredService<ICurrentTenant>().IsAvailable
            ? typeof(ProTenantDbContext)
            : typeof(ProDbContext);

        await ((DbContext)_serviceProvider.GetRequiredService(dbContextType))
            .Database
            .MigrateAsync();
    }
}
