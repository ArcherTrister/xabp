// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Reflection;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Volo.Abp;
using Volo.Abp.Modularity;
using Volo.Abp.Quartz;

using X.Abp.Quartz.SqlServer;

namespace X.Abp.Quartz;

[DependsOn(typeof(AbpQuartzDomainModule))]
public class AbpQuartzInstallScriptSqlServerModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpQuartzInstallScriptOptions>(options =>
        {
            options.ScriptAssembly = typeof(SqlServerObjectsInstaller).GetTypeInfo().Assembly;
            options.ScriptResourceName = "X.Abp.Quartz.SqlServer.Install.sql";
        });
    }

    public override async Task OnPreApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        var options = context.ServiceProvider.GetRequiredService<IOptions<AbpQuartzOptions>>().Value;

        var installScriptOptions = context.ServiceProvider.GetRequiredService<IOptions<AbpQuartzInstallScriptOptions>>().Value;

        var objectsInstaller = context.ServiceProvider.GetRequiredService<IObjectsInstaller>();

        await objectsInstaller.Initialize(options, installScriptOptions);
    }
}
