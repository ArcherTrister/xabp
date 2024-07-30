// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Volo.Abp;
using Volo.Abp.Modularity;
using Volo.Abp.Quartz;

namespace X.Abp.Quartz;

[DependsOn(typeof(AbpQuartzModule))]
public class AbpQuartzInstallScriptSqlServerModule : AbpModule
{
    public override async Task OnPreApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        var options = context.ServiceProvider.GetRequiredService<IOptions<AbpQuartzOptions>>().Value;

        var objectsInstaller = context.ServiceProvider.GetRequiredService<IObjectsInstaller>();

        await objectsInstaller.Initialize(options);
    }
}
