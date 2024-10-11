// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AbpVnext.Pro.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */
public abstract class ProDbContextFactoryBase<TDbContext> : IDesignTimeDbContextFactory<TDbContext>
    where TDbContext : DbContext
{
    public TDbContext CreateDbContext(string[] args)
    {
        ProEfCoreEntityExtensionMappings.Configure();

        Console.WriteLine("添加环境变量参数格式: -e RuntimeEnvironment 或 --environment RuntimeEnvironment");
        Console.WriteLine("** RuntimeEnvironment ** 可选参数: [Development | Production | Staging]");
        Console.WriteLine("** Add-Migration/dotnet ef migrations add ** 不受环境变量影响");
        Console.WriteLine("① Nuget包管理器控制台");
        Console.WriteLine("Add-Migration MigrationName");
        Console.WriteLine("Update-Database -Args '--environment Development'");
        Console.WriteLine("Remove-Migration -Args '--environment Development'");
        Console.WriteLine("② 命令提示符");
        Console.WriteLine("dotnet ef migrations add MigrationName");
        Console.WriteLine("dotnet ef database update -- --environment Development");
        Console.WriteLine("dotnet ef migrations remove -- --environment Development");

        // Development Production Staging
        string environment = string.Empty;
        var index = args.FindIndex(p => p.Equals("-e", StringComparison.OrdinalIgnoreCase) || p.Equals("--environment", StringComparison.OrdinalIgnoreCase));
        if (index > -1)
        {
            try
            {
                environment = (string)args.GetValue(index + 1);
            }
            catch (Exception)
            {
            }
        }

        var configuration = BuildConfiguration(environment);

        var builder = new DbContextOptionsBuilder<TDbContext>()
            .UseMySql(configuration.GetConnectionString("Default"), MySqlServerVersion.LatestSupportedServerVersion);

        return CreateDbContext(builder.Options);
    }

    protected abstract TDbContext CreateDbContext(DbContextOptions<TDbContext> dbContextOptions);

    protected IConfigurationRoot BuildConfiguration(string environment)
    {
        Console.WriteLine($"当前运行环境: {(environment.IsNullOrWhiteSpace() ? "未设置, 将使用默认配置" : environment)}, 如需更改运行环境请在运行命令中指定");

        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../AbpVnext.Pro.DbMigrator/"))
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
