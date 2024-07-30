// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Data;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using MySql.Data.MySqlClient;

using Quartz.Impl.AdoJobStore;

using Volo.Abp.DependencyInjection;
using Volo.Abp.Quartz;

using static Quartz.SchedulerBuilder;

namespace X.Abp.Quartz.MySql;

public class MySqlObjectsInstaller : IObjectsInstaller, ISingletonDependency
{
    protected ILogger<MySqlObjectsInstaller> Logger { get; set; }

    public MySqlObjectsInstaller()
    {
        Logger = NullLogger<MySqlObjectsInstaller>.Instance;
    }

    public async Task Initialize(AbpQuartzOptions options)
    {
        // AdoProviderOptions
        var connectionString = options.Properties[$"quartz.dataSource.{AdoProviderOptions.DefaultDataSourceName}.connectionString"];
        var tablePrefix = options.Properties["quartz.jobStore.tablePrefix"];

        if (connectionString.IsNullOrWhiteSpace())
        {
            throw new ArgumentNullException(nameof(connectionString));
        }

        Logger.LogInformation("Start installing Quartz SQL objects...");

        var builder = new MySqlConnectionStringBuilder(connectionString);
        var dataBaseName = builder.Database;
        builder.Remove("Database");

        using (var connection = new MySqlConnection(builder.ConnectionString))
        {
            if (connection.State == ConnectionState.Closed)
            {
                await connection.OpenAsync();
            }

            string checkDataBaseExists;

            using (var cmd = new MySqlCommand("SELECT SCHEMA_NAME FROM information_schema.SCHEMATA WHERE SCHEMA_NAME=@dataBaseName;", connection))
            {
                cmd.Parameters.Add("@dataBaseName", MySqlDbType.String).Value = dataBaseName;
                checkDataBaseExists = await cmd.ExecuteScalarAsync() as string;
            }

            if (checkDataBaseExists.IsNullOrWhiteSpace())
            {
                using (var cmd = new MySqlCommand($"CREATE DATABASE {dataBaseName} DEFAULT CHARACTER SET utf8 COLLATE utf8_general_ci;", connection))
                {
                    await cmd.ExecuteScalarAsync();
                }

                var sql = GetInstallScript(dataBaseName, tablePrefix, true);

                using (var cmd = new MySqlCommand(sql, connection))
                {
                    await cmd.ExecuteScalarAsync();
                }
            }
            else
            {
                long? checkTablesExists;

                using (var cmd = new MySqlCommand("SELECT count(1) FROM information_schema.TABLES WHERE TABLE_SCHEMA = @dataBaseName;", connection))
                {
                    cmd.Parameters.Add("@dataBaseName", MySqlDbType.String).Value = dataBaseName;
                    checkTablesExists = (long?)await cmd.ExecuteScalarAsync();
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

                    using var cmd = new MySqlCommand(sql, connection);
                    await cmd.ExecuteScalarAsync();
                }
            }

            await connection.CloseAsync();
        }

        Logger.LogInformation("Quartz SQL objects installed.");
    }

    private string GetInstallScript(string dataBaseName, string tablePrefix, bool enableHeavyMigrations)
    {
        var script = GetStringResource(
            typeof(MySqlObjectsInstaller).GetTypeInfo().Assembly,
            "X.Abp.Quartz.MySql.Install.sql");

        script = script.Replace("$(TablePrefix)", !string.IsNullOrWhiteSpace(tablePrefix) ? tablePrefix : AdoConstants.DefaultTablePrefix)
            .ReplaceFirst("$(QuartzSchema)", dataBaseName);

        if (!enableHeavyMigrations)
        {
            script = script.Replace("--SET @DISABLE_HEAVY_MIGRATIONS = 1;", "SET @DISABLE_HEAVY_MIGRATIONS = 1;");
        }

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
