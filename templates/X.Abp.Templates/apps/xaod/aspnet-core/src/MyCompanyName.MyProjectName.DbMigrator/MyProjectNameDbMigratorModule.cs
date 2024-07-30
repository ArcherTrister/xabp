// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MyCompanyName.MyProjectName.EntityFrameworkCore;

using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;

namespace MyCompanyName.MyProjectName.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(MyProjectNameEntityFrameworkCoreModule),
    typeof(MyProjectNameApplicationContractsModule)
)]
public class MyProjectNameDbMigratorModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpBackgroundJobOptions>(options =>
        {
            options.IsJobExecutionEnabled = false;
        });
    }
}
