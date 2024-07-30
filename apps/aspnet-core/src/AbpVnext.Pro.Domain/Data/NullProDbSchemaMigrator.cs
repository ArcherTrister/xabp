// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Volo.Abp.DependencyInjection;

namespace AbpVnext.Pro.Data;

/* This is used if database provider does't define
 * IProDbSchemaMigrator implementation.
 */
public class NullProDbSchemaMigrator : IProDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
