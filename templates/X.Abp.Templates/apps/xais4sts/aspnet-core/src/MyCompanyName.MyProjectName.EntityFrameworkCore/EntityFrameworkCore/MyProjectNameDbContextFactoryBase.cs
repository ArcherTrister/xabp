// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

using System.IO;

namespace MyCompanyName.MyProjectName.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */
public abstract class MyProjectNameDbContextFactoryBase<TDbContext> : IDesignTimeDbContextFactory<TDbContext>
    where TDbContext : DbContext
{
    public TDbContext CreateDbContext(string[] args)
    {
#if PostgreSql
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
#endif
        MyProjectNameEfCoreEntityExtensionMappings.Configure();

        var configuration = BuildConfiguration();

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
        // builder.EnableSensitiveDataLogging();

        return CreateDbContext(builder.Options);
    }

    protected abstract TDbContext CreateDbContext(DbContextOptions<TDbContext> dbContextOptions);

    protected IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../MyCompanyName.MyProjectName.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
