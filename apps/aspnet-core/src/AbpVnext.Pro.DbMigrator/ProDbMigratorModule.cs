// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using AbpVnext.Pro.EntityFrameworkCore;

using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;

namespace AbpVnext.Pro.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(ProEntityFrameworkCoreModule),
    typeof(ProApplicationContractsModule))]
public class ProDbMigratorModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpBackgroundJobOptions>(options =>
        {
            options.IsJobExecutionEnabled = false;
        });
    }
}
