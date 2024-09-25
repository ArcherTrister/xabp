using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace MyCompanyName.MyProjectName.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */
public class MyProjectNameDbContextFactory : IDesignTimeDbContextFactory<MyProjectNameDbContext>
{
    public MyProjectNameDbContext CreateDbContext(string[] args)
    {
#if PostgreSql
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
#endif
        MyProjectNameEfCoreEntityExtensionMappings.Configure();

        System.Console.WriteLine("添加环境变量参数格式: -e RuntimeEnvironment 或 --environment RuntimeEnvironment");
        System.Console.WriteLine("** RuntimeEnvironment ** 可选参数: [Development | Production | Staging]");
        System.Console.WriteLine("① Nuget包管理器控制台");
        System.Console.WriteLine("Add-Migration MigrationName -Args '--environment Development'");
        System.Console.WriteLine("Update-Database -Args '--environment Development'");
        System.Console.WriteLine("Remove-Migration -Args '--environment Development'");
        System.Console.WriteLine("② 命令提示符");
        System.Console.WriteLine("dotnet ef migrations add MigrationName -- --environment Development");
        System.Console.WriteLine("dotnet ef database update -- --environment Development");
        System.Console.WriteLine("dotnet ef migrations remove -- --environment Development");

        // Development Production Staging
        string environment = string.Empty;
        var index = args.FindIndex(p => p.Equals("-e", System.StringComparison.OrdinalIgnoreCase) || p.Equals("--environment", System.StringComparison.OrdinalIgnoreCase));
        if (index > -1)
        {
            try
            {
                environment = (string)args.GetValue(index + 1);
            }
            catch (System.Exception)
            {
            }
        }

        var configuration = BuildConfiguration(environment);

        var builder = new DbContextOptionsBuilder<MyProjectNameDbContext>()
#if MySQL
            .UseMySql(configuration.GetConnectionString("Default"), MySqlServerVersion.LatestSupportedServerVersion);
#elif SQLServer
            .UseSqlServer(configuration.GetConnectionString("Default"));
#elif SQLite
            .UseSqlite(configuration.GetConnectionString("Default"));
#elif Oracle
            .UseOracle(configuration.GetConnectionString("Default"));
#elif OracleDevart
            .UseOracle(configuration.GetConnectionString("Default"));
#else
            .UseNpgsql(configuration.GetConnectionString("Default"));
#endif

        return new MyProjectNameDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration(string? environment)
    {
        System.Console.WriteLine($"当前运行环境: {(environment.IsNullOrWhiteSpace() ? "未设置, 将使用默认配置" : environment)}, 如需更改运行环境请在运行命令中指定");

        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../MyCompanyName.MyProjectName.DbMigrator/"))
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
