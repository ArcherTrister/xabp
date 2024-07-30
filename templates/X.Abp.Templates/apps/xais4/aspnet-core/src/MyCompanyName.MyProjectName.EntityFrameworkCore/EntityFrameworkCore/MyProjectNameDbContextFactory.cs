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
public class MyProjectNameDbContextFactory : IDesignTimeDbContextFactory<MyProjectNameDbContext>
{
    public MyProjectNameDbContext CreateDbContext(string[] args)
    {
        MyProjectNameEfCoreEntityExtensionMappings.Configure();

        var configuration = BuildConfiguration();

#if MySQL
        var builder = new DbContextOptionsBuilder<MyProjectNameDbContext>()
            .UseMySql(configuration.GetConnectionString("Default"), MySqlServerVersion.LatestSupportedServerVersion);
#elif SqlServer
        var builder = new DbContextOptionsBuilder<MyProjectNameDbContext>()
            .UseSqlServer(configuration.GetConnectionString("Default"));
#elif Sqlite
        var builder = new DbContextOptionsBuilder<MyProjectNameDbContext>()
            .UseSqlite(configuration.GetConnectionString("Default"));
#elif Oracle
        var builder = new DbContextOptionsBuilder<MyProjectNameDbContext>()
            .UseOracle(configuration.GetConnectionString("Default"));
#elif OracleDevart
        var builder = new DbContextOptionsBuilder<MyProjectNameDbContext>()
            .UseOracle(configuration.GetConnectionString("Default"));
#elif PostgreSql
        var builder = new DbContextOptionsBuilder<MyProjectNameDbContext>()
            .UseNpgsql(configuration.GetConnectionString("Default"));
#endif
        // builder.EnableSensitiveDataLogging();

        return new MyProjectNameDbContext(builder.Options);
    }

    protected IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../MyCompanyName.MyProjectName.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
