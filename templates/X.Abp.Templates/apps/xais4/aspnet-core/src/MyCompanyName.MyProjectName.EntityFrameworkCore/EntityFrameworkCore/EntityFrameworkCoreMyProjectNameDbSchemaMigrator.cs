// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MyCompanyName.MyProjectName.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Threading.Tasks;

using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace MyCompanyName.MyProjectName.EntityFrameworkCore;

public class EntityFrameworkCoreMyProjectNameDbSchemaMigrator
    : IMyProjectNameDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreMyProjectNameDbSchemaMigrator(IServiceProvider serviceProvider)
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

        await _serviceProvider
            .GetRequiredService<MyProjectNameDbContext>()
            .Database
            .MigrateAsync();
    }
}
