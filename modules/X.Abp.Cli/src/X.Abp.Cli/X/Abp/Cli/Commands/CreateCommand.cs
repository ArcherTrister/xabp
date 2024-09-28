// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Volo.Abp.Cli;
using Volo.Abp.Cli.Args;
using Volo.Abp.Cli.Commands;
using Volo.Abp.Cli.Commands.Services;
using Volo.Abp.Cli.ProjectBuilding;
using Volo.Abp.Cli.ProjectBuilding.Building;
using Volo.Abp.Cli.Utils;
using Volo.Abp.DependencyInjection;
using Volo.Abp.IO;

using X.Abp.Cli.ProjectBuilding;

namespace X.Abp.Cli.Commands;

public class CreateCommand : IConsoleCommand, ITransientDependency
{
  public const string Name = "create";

  protected static string CommandName => Name;

  protected ILogger<CreateCommand> Logger { get; set; }

  protected ConnectionStringProvider ConnectionStringProvider { get; }

  protected ICreateProjectService CreateProjectService { get; }

  public CreateCommand(
      ICreateProjectService createProjectService,
      ConnectionStringProvider connectionStringProvider,
      ILogger<CreateCommand> logger)
  {
    CreateProjectService = createProjectService;
    ConnectionStringProvider = connectionStringProvider;

    Logger = logger ?? NullLogger<CreateCommand>.Instance;
  }

  public virtual async Task ExecuteAsync(CommandLineArgs commandLineArgs)
  {
    var projectName = NamespaceHelper.NormalizeNamespace(commandLineArgs.Target);
    if (string.IsNullOrWhiteSpace(projectName))
    {
      throw new CliUsageException("Project name is missing!" + Environment.NewLine + Environment.NewLine + GetUsageInfo());
    }

    ProjectNameValidator.Validate(projectName);

    Logger.LogInformation("Creating your project...");
    Logger.LogInformation("Project name: " + projectName);

    var templateName = commandLineArgs.Options.GetOrNull(NewCommand.Options.Template.Short, NewCommand.Options.Template.Long);
    if (templateName != null)
    {
      Logger.LogInformation("Template: " + templateName);
    }
    else
    {
      templateName = "xais4";
    }

    var version = commandLineArgs.Options.GetOrNull(NewCommand.Options.Version.Short, NewCommand.Options.Version.Long);

    if (version != null)
    {
      Logger.LogInformation("Version: " + version);
    }

    var databaseProvider = GetDatabaseProvider(commandLineArgs);
    if (databaseProvider != DatabaseProvider.NotSpecified)
    {
      Logger.LogInformation("Database provider: " + databaseProvider);
    }

    var connectionString = GetConnectionString(commandLineArgs);
    if (connectionString != null)
    {
      Logger.LogInformation("Connection string: " + connectionString);
    }

    var databaseManagementSystem = GetDatabaseManagementSystem(commandLineArgs);
    if (databaseManagementSystem != DatabaseManagementSystem.NotSpecified)
    {
      Logger.LogInformation("DBMS: " + databaseManagementSystem);
    }

    var createSolutionFolder = GetCreateSolutionFolderPreference(commandLineArgs);

    var outputFolder = commandLineArgs.Options.GetOrNull(NewCommand.Options.OutputFolder.Short, NewCommand.Options.OutputFolder.Long);

    var outputFolderRoot =
        outputFolder != null ? Path.GetFullPath(outputFolder) : Directory.GetCurrentDirectory();

    var solutionName = SolutionName.Parse(projectName);

    outputFolder = createSolutionFolder ?
        Path.Combine(outputFolderRoot, SolutionName.Parse(projectName).FullName) :
        outputFolderRoot;

    DirectoryHelper.CreateIfNotExists(outputFolder);

    Logger.LogInformation("Output folder: " + outputFolder);

    if (connectionString == null &&
       databaseManagementSystem != DatabaseManagementSystem.NotSpecified &&
       databaseManagementSystem != DatabaseManagementSystem.SQLServer)
    {
      connectionString = ConnectionStringProvider.GetByDbms(databaseManagementSystem, outputFolder);
    }

    commandLineArgs.Options.Add(CliConsts.Command, commandLineArgs.Command);

    var projectArgs = new ProjectBuildArgs(
        solutionName,
        templateName,
        version,
        outputFolder,
        databaseProvider,
        databaseManagementSystem,
        UiFramework.None,
        null,
        false,
        null,
        null,
        null,
        commandLineArgs.Options,
        connectionString);

    await CreateProjectService.CreateAsync(projectArgs);
  }

  public static string GetShortDescription()
  {
    return "Generate a new solution based on the ABPVnext Pro startup templates.";
  }

  public string GetUsageInfo()
  {
    var sb = new StringBuilder();

    sb.AppendLine(string.Empty);
    sb.AppendLine("Usage:");
    sb.AppendLine(string.Empty);
    sb.AppendLine("  xabp create <project-name> [options]");
    sb.AppendLine(string.Empty);
    sb.AppendLine("Options:");
    sb.AppendLine(string.Empty);
    sb.AppendLine("-t|--template <template-name>               (default: xais4)");
    sb.AppendLine("-tt|--template-type <template-type>               (default: IdentityServer4)");
    sb.AppendLine("-o|--output-folder <output-folder>          (default: current folder)");
    sb.AppendLine("-v|--version <version>                      (default: latest version)");
    sb.AppendLine("-cs|--connection-string <connection-string> (your database connection string)");
    sb.AppendLine("-dbms|--database-management-system <database-management-system>         (your database management system, default: SqlServer.)");
    sb.AppendLine("-iv|--include-vue <include-vue>             (default: false)");
    sb.AppendLine("-esef|--enable-swagger-enum-filter          (default: true)");
    sb.AppendLine("-csf|--create-solution-folder               (default: true)");
    sb.AppendLine("-it|--install-template <install-template>             (default: false)");
    sb.AppendLine(string.Empty);
    sb.AppendLine("Examples:");
    sb.AppendLine(string.Empty);
    sb.AppendLine("  xabp create Acme.BookStore");
    sb.AppendLine("  xabp create Acme.BookStore -t xais4");
    sb.AppendLine("  xabp create Acme.BookStore -t xaod");
    sb.AppendLine("  xabp create Acme.BookStore -csf false");
    sb.AppendLine("  xabp create Acme.BookStore --dbms mysql");
    sb.AppendLine("  xabp create Acme.BookStore --connection-string \"Server=myServerName\\myInstanceName;Database=myDatabase;User Id=myUsername;Password=myPassword\"");
    sb.AppendLine(string.Empty);
    sb.AppendLine("See the documentation for more info: https://docs.abp.io/zh-Hans/abp/latest/CLI#new");

    return sb.ToString();
  }

  protected static bool GetCreateSolutionFolderPreference(CommandLineArgs commandLineArgs)
  {
    if (!commandLineArgs.Options.TryGetValue(NewCommand.Options.CreateSolutionFolder.Long, out var createSolutionFolder))
    {
      if (!commandLineArgs.Options.TryGetValue(NewCommand.Options.CreateSolutionFolder.Short, out createSolutionFolder))
      {
        createSolutionFolder = "true";
      }
    }

    return createSolutionFolder.Equals("true", StringComparison.OrdinalIgnoreCase);
  }

  protected virtual DatabaseProvider GetDatabaseProvider(CommandLineArgs commandLineArgs)
  {
    var optionValue = commandLineArgs.Options.GetOrNull(
        NewCommand.Options.DatabaseProvider.Short,
        NewCommand.Options.DatabaseProvider.Long);
    return optionValue switch
    {
      "ef" => DatabaseProvider.EntityFrameworkCore,
      "mongodb" => DatabaseProvider.MongoDb,
      _ => DatabaseProvider.NotSpecified,
    };
  }

  protected virtual DatabaseManagementSystem GetDatabaseManagementSystem(CommandLineArgs commandLineArgs)
  {
    var optionValue = commandLineArgs.Options.GetOrNull(
        NewCommand.Options.DatabaseManagementSystem.Short,
        NewCommand.Options.DatabaseManagementSystem.Long);

    return optionValue == null
        ? DatabaseManagementSystem.NotSpecified
        : optionValue.ToLowerInvariant() switch
        {
          "sqlserver" => DatabaseManagementSystem.SQLServer,
          "mysql" => DatabaseManagementSystem.MySQL,
          "postgresql" => DatabaseManagementSystem.PostgreSQL,
          "oracle-devart" => DatabaseManagementSystem.OracleDevart,
          "sqlite" => DatabaseManagementSystem.SQLite,
          "oracle" => DatabaseManagementSystem.Oracle,
          _ => DatabaseManagementSystem.NotSpecified,
        };
  }

  protected static string GetConnectionString(CommandLineArgs commandLineArgs)
  {
    var connectionString = commandLineArgs.Options.GetOrNull(
        NewCommand.Options.ConnectionString.Short,
        NewCommand.Options.ConnectionString.Long);
    return string.IsNullOrWhiteSpace(connectionString) ? null : connectionString;
  }
}
