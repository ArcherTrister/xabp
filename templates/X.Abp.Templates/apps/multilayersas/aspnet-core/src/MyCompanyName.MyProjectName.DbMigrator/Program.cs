using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Serilog;
using Serilog.Events;

namespace MyCompanyName.MyProjectName.DbMigrator;

class Program
{
    static async Task Main(string[] args)
    {
        // Development Production Staging
        // Environment.SetEnvironmentVariable("RuntimeEnvironment", "Development");
        // Environment.SetEnvironmentVariable("RuntimeEnvironment", "Production");
        // Environment.SetEnvironmentVariable("RuntimeEnvironment", "Staging");
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Volo.Abp", LogEventLevel.Warning)
#if DEBUG
                .MinimumLevel.Override("MyCompanyName.MyProjectName", LogEventLevel.Debug)
#else
                .MinimumLevel.Override("MyCompanyName.MyProjectName", LogEventLevel.Information)
#endif
                .Enrich.FromLogContext()
            .WriteTo.Async(c => c.File("Logs/logs.txt"))
            .WriteTo.Async(c => c.Console())
            .CreateLogger();

        await CreateHostBuilder(args).RunConsoleAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .AddAppSettingsSecretsJson()
            .ConfigureLogging((context, logging) => logging.ClearProviders())
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<DbMigratorHostedService>();
            });
}
