// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;

using Volo.Abp.DependencyInjection;

namespace MyCompanyName.MyProjectName.Data;

/* This is used if database provider does't define
 * IMyProjectNameDbSchemaMigrator implementation.
 */
public class NullMyProjectNameDbSchemaMigrator : IMyProjectNameDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
