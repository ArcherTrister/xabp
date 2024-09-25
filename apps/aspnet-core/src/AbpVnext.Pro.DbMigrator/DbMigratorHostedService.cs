// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading;
using System.Threading.Tasks;

using AbpVnext.Pro.Data;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;

using Volo.Abp;
using Volo.Abp.Data;

namespace AbpVnext.Pro.DbMigrator;

public class DbMigratorHostedService : IHostedService
{
    private readonly IHostApplicationLifetime _hostApplicationLifetime;

    public DbMigratorHostedService(IHostApplicationLifetime hostApplicationLifetime)
    {
        _hostApplicationLifetime = hostApplicationLifetime;
    }

    public virtual async Task StartAsync(CancellationToken cancellationToken)
    {
        using var application = await AbpApplicationFactory.CreateAsync<ProDbMigratorModule>(options =>
        {
            options.Services.ReplaceConfiguration(BuildConfiguration());
            options.UseAutofac();
            options.Services.AddLogging(c => c.AddSerilog());
            options.AddDataMigrationEnvironment();
        });
        await application.InitializeAsync();

        await application
            .ServiceProvider
            .GetRequiredService<ProDbMigrationService>()
            .MigrateAsync();

        await application.ShutdownAsync();

        _hostApplicationLifetime.StopApplication();
    }

    public virtual Task StopAsync(CancellationToken cancellationToken)
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

        Console.WriteLine($"当前运行环境: {(environment.IsNullOrWhiteSpace() ? "未设置, 将使用默认配置" : environment)}, 如需更改运行环境请在Main函数中修改");
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.secrets.json", optional: false);

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
