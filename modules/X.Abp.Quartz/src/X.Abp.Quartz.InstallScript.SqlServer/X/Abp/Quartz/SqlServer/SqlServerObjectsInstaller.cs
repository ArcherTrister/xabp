// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Data;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Quartz.Impl.AdoJobStore;

using Volo.Abp.DependencyInjection;
using Volo.Abp.Quartz;

using static Quartz.SchedulerBuilder;

namespace X.Abp.Quartz.SqlServer;

public class SqlServerObjectsInstaller : IObjectsInstaller, ISingletonDependency
{
    protected ILogger<SqlServerObjectsInstaller> Logger { get; set; }

    public SqlServerObjectsInstaller()
    {
        Logger = NullLogger<SqlServerObjectsInstaller>.Instance;
    }

    public async Task Initialize(AbpQuartzOptions options)
    {
        var connectionString = options.Properties[$"quartz.dataSource.{AdoProviderOptions.DefaultDataSourceName}.connectionString"];
        var tablePrefix = options.Properties["quartz.jobStore.tablePrefix"];

        if (connectionString.IsNullOrWhiteSpace())
        {
            throw new ArgumentNullException(nameof(connectionString));
        }

        Logger.LogInformation("Start installing Quartz SQL objects...");

        var builder = new SqlConnectionStringBuilder(connectionString);
        var dataBaseName = builder.InitialCatalog;
        builder.Remove("Initial Catalog");

        using (var connection = new SqlConnection(builder.ConnectionString))
        {
            if (connection.State == ConnectionState.Closed)
            {
                await connection.OpenAsync();
            }

            string checkDataBaseExists;

            using (var cmd = new SqlCommand("SELECT [name] FROM [master].[dbo].[sysdatabases] WHERE [name] = @dataBaseName;", connection))
            {
                cmd.Parameters.Add("@dataBaseName", SqlDbType.NChar).Value = dataBaseName;
                checkDataBaseExists = await cmd.ExecuteScalarAsync() as string;
            }

            if (checkDataBaseExists.IsNullOrWhiteSpace())
            {
                using (var cmd = new SqlCommand($"CREATE DATABASE {dataBaseName};", connection))
                {
                    await cmd.ExecuteScalarAsync();
                }

                var sql = GetInstallScript(dataBaseName, tablePrefix, true);

                using (var cmd = new SqlCommand(sql, connection))
                {
                    await cmd.ExecuteScalarAsync();
                }
            }
            else
            {
                int? checkTablesExists;

                using (var cmd = new SqlCommand($"USE {dataBaseName};SELECT count(1) FROM [sys].[objects] WHERE type='U';", connection))
                {
                    checkTablesExists = (int?)await cmd.ExecuteScalarAsync();
                }

                if (checkTablesExists is not null and > 0)
                {
                    if (checkTablesExists == 12)
                    {
                        Logger.LogInformation("DB tables already exist. Exit install.");
                    }
                    else
                    {
                        Logger.LogError($"DB tables already exist. Exit install.But the number of tables is not enough.");
                    }
                }
                else
                {
                    var sql = GetInstallScript(dataBaseName, tablePrefix, true);

                    using var cmd = new SqlCommand(sql, connection);
                    await cmd.ExecuteScalarAsync();
                }
            }

            connection.Close();
        }

        Logger.LogInformation("Quartz SQL objects installed.");
    }

    private string GetInstallScript(string dataBaseName, string tablePrefix, bool enableHeavyMigrations)
    {
        var script = GetStringResource(
            typeof(SqlServerObjectsInstaller).GetTypeInfo().Assembly,
            "X.Abp.Quartz.SqlServer.Install.sql");

        script = script.Replace("$(TablePrefix)", !string.IsNullOrWhiteSpace(tablePrefix) ? tablePrefix : AdoConstants.DefaultTablePrefix)
            .ReplaceFirst("$(QuartzSchema)", dataBaseName);

        // if (!enableHeavyMigrations)
        // {
        //     script = script.Replace("--SET @DISABLE_HEAVY_MIGRATIONS = 1;", "SET @DISABLE_HEAVY_MIGRATIONS = 1;");
        // }
        return script;
    }

    /// <summary>
    /// 嵌入式资源转字符串
    /// </summary>
    /// <param name="assembly">assembly</param>
    /// <param name="resourceName">资源名称【全称】</param>
    private string GetStringResource(Assembly assembly, string resourceName)
    {
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            throw new InvalidOperationException(
                $"Requested resource `{resourceName}` was not found in the assembly `{assembly}`.");
        }

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
