// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Serilog;
using Serilog.Events;

namespace AbpVnext.Pro.DbMigrator;

internal class Program
{
    private static async Task Main(string[] args)
    {
        // Development Production Staging
        // Environment.SetEnvironmentVariable("RuntimeEnvironment", "Development");
        // Environment.SetEnvironmentVariable("RuntimeEnvironment", "Production");
        // Environment.SetEnvironmentVariable("RuntimeEnvironment", "Staging");
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Volo.Abp", LogEventLevel.Warning)
            .MinimumLevel.Override("X.Abp", LogEventLevel.Warning)
#if DEBUG
                .MinimumLevel.Override("AbpVnext.Pro", LogEventLevel.Debug)
#else
                .MinimumLevel.Override("AbpVnext.Pro", LogEventLevel.Information)
#endif
                .Enrich.FromLogContext()
            .WriteTo.Async(c => c.File("Logs/logs.txt"))
            .WriteTo.Async(c => c.Console())
            .CreateLogger();

        await CreateHostBuilder(args).RunConsoleAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .AddAppSettingsSecretsJson()
            .ConfigureLogging((context, logging) => logging.ClearProviders())
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<DbMigratorHostedService>();
            });
    }
}
