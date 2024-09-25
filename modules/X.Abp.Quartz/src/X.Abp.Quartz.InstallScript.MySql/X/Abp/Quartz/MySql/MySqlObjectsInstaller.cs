// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Data;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using MySql.Data.MySqlClient;

using Volo.Abp.DependencyInjection;
using Volo.Abp.Quartz;

using static Quartz.SchedulerBuilder;

namespace X.Abp.Quartz.MySql;

[Dependency(ReplaceServices = true)]
public class MySqlObjectsInstaller : ObjectsInstallerBase
{
  protected ILogger<MySqlObjectsInstaller> Logger { get; set; }

  public MySqlObjectsInstaller()
  {
    Logger = NullLogger<MySqlObjectsInstaller>.Instance;
  }

  public override async Task Initialize(AbpQuartzOptions options, AbpQuartzInstallScriptOptions installScriptOptions)
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

        var sql = GetInstallScript(installScriptOptions.ScriptAssembly, installScriptOptions.ScriptResourceName, dataBaseName, tablePrefix, installScriptOptions.EnableHeavyMigrations);

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
            Logger.LogError("DB tables already exist. Exit install.But the number of tables is not enough.");
          }
        }
        else
        {
          var sql = GetInstallScript(installScriptOptions.ScriptAssembly, installScriptOptions.ScriptResourceName, dataBaseName, tablePrefix, installScriptOptions.EnableHeavyMigrations);

          using var cmd = new MySqlCommand(sql, connection);
          await cmd.ExecuteScalarAsync();
        }
      }

      await connection.CloseAsync();
    }

    Logger.LogInformation("Quartz SQL objects installed.");
  }
}
