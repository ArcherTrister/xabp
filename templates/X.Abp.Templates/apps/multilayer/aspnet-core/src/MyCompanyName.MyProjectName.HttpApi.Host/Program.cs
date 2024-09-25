using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;
using Serilog.Events;

namespace MyCompanyName.MyProjectName;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
#if DEBUG
            .MinimumLevel.Debug()
#else
            .MinimumLevel.Information()
#endif
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Logger(c => c.Filter.ByIncludingOnly(p => p.Level == LogEventLevel.Verbose).WriteTo.Async(q => q.File("Logs/verboses_.txt", rollingInterval: RollingInterval.Day)))
            .WriteTo.Logger(c => c.Filter.ByIncludingOnly(p => p.Level == LogEventLevel.Debug).WriteTo.Async(q => q.File("Logs/debugs_.txt", rollingInterval: RollingInterval.Day)))
            .WriteTo.Logger(c => c.Filter.ByIncludingOnly(p => p.Level == LogEventLevel.Information).WriteTo.Async(q => q.File("Logs/infos_.txt", rollingInterval: RollingInterval.Day)))
            .WriteTo.Logger(c => c.Filter.ByIncludingOnly(p => p.Level == LogEventLevel.Warning).WriteTo.Async(q => q.File("Logs/warnings_.txt", rollingInterval: RollingInterval.Day)))
            .WriteTo.Logger(c => c.Filter.ByIncludingOnly(p => p.Level == LogEventLevel.Error).WriteTo.Async(q => q.File("Logs/errors_.txt", rollingInterval: RollingInterval.Day)))
            .WriteTo.Logger(c => c.Filter.ByIncludingOnly(p => p.Level == LogEventLevel.Fatal).WriteTo.Async(q => q.File("Logs/fatals_.txt", rollingInterval: RollingInterval.Day)))
            .WriteTo.Async(c => c.Console())
            .CreateLogger();

        try
        {
            Log.Information("Starting MyCompanyName.MyProjectName.HttpApi.Host.");
            var builder = WebApplication.CreateBuilder(args);
            builder.Host.AddAppSettingsSecretsJson()
                .UseAutofac()
                .UseSerilog();
            await builder.AddApplicationAsync<MyProjectNameHttpApiHostModule>();
            var app = builder.Build();
            await app.InitializeApplicationAsync();
            await app.RunAsync();
            return 0;
        }
        catch (Exception ex)
        {
            if (ex is HostAbortedException)
            {
                throw;
            }

            Log.Fatal(ex, "Host terminated unexpectedly!");
            return 1;
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }
}
