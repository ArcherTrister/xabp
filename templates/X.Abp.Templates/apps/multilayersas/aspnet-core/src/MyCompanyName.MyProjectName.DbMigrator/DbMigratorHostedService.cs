using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using MyCompanyName.MyProjectName.Data;

using Serilog;

using Volo.Abp;
using Volo.Abp.Data;

namespace MyCompanyName.MyProjectName.DbMigrator;

public class DbMigratorHostedService : IHostedService
{
    private readonly IHostApplicationLifetime _hostApplicationLifetime;

    public DbMigratorHostedService(IHostApplicationLifetime hostApplicationLifetime)
    {
        _hostApplicationLifetime = hostApplicationLifetime;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using (var application = await AbpApplicationFactory.CreateAsync<MyProjectNameDbMigratorModule>(options =>
        {
            options.Services.ReplaceConfiguration(BuildConfiguration());
            options.UseAutofac();
            options.Services.AddLogging(c => c.AddSerilog());
            options.AddDataMigrationEnvironment();
        }))
        {
            await application.InitializeAsync();

            await application
                .ServiceProvider
                .GetRequiredService<MyProjectNameDbMigrationService>()
                .MigrateAsync();

            await application.ShutdownAsync();

            _hostApplicationLifetime.StopApplication();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        string environment = string.Empty;
        try
        {
            environment = Environment.GetEnvironmentVariable("RuntimeEnvironment");
        }
        catch (Exception)
        {
        }

        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.secrets.json", optional: false);

        System.Console.WriteLine($"当前运行环境: {(environment.IsNullOrWhiteSpace() ? "未设置, 将使用默认配置" : environment)}, 如需更改运行环境请在Main函数中修改");
        if (environment.IsNullOrWhiteSpace())
        {
            builder.AddJsonFile("appsettings.json", optional: false);
        }
        else
        {
            builder.AddJsonFile($"appsettings.{environment}.json", optional: false);
        }

        return builder.Build();
    }
}
