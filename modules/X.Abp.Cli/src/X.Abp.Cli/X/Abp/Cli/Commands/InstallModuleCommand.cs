// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

using Volo.Abp.Cli;
using Volo.Abp.Cli.Args;
using Volo.Abp.Cli.Commands;
using Volo.Abp.Cli.ProjectBuilding.Templates.MvcModule;
using Volo.Abp.Cli.ProjectModification;
using Volo.Abp.Cli.Utils;
using Volo.Abp.DependencyInjection;

namespace X.Abp.Cli.Commands;
public class InstallModuleCommand : IConsoleCommand, ITransientDependency
{
    private readonly AbpCliOptions _options;
    public const string Name = "install-module";

    private AddModuleInfoOutput _lastAddedModuleInfo;

    protected ILogger<AddModuleCommand> Logger { get; set; }

    protected SolutionModuleAdder SolutionModuleAdder { get; }

    protected SolutionPackageVersionFinder SolutionPackageVersionFinder { get; }

    public AddModuleInfoOutput LastAddedModuleInfo
    {
        get
        {
            if (_lastAddedModuleInfo == null)
            {
#pragma warning disable CA1065 // 不要在意外的位置引发异常
                throw new Exception("You need to add a module first to get the last added module info!");
#pragma warning restore CA1065 // 不要在意外的位置引发异常
            }

            return _lastAddedModuleInfo;
        }
    }

    public InstallModuleCommand(
        SolutionModuleAdder solutionModuleAdder,
        SolutionPackageVersionFinder solutionPackageVersionFinder,
        IOptions<AbpCliOptions> options,
        ILogger<AddModuleCommand> logger)
    {
        SolutionModuleAdder = solutionModuleAdder;
        SolutionPackageVersionFinder = solutionPackageVersionFinder;
        _options = options.Value;
        Logger = logger ?? NullLogger<AddModuleCommand>.Instance;
    }

    public async Task ExecuteAsync(CommandLineArgs commandLineArgs)
    {
        if (commandLineArgs.Target == null)
        {
            throw new CliUsageException(
                "Module name is missing!" +
                Environment.NewLine + Environment.NewLine +
                GetUsageInfo());
        }

        if (_options.DisabledModulesToAddToSolution.Contains(commandLineArgs.Target))
        {
            throw new CliUsageException(
                $"{commandLineArgs.Target} Module is not available for this command! You can check the module's documentation for more info.");
        }

        var newTemplate = commandLineArgs.Options.TryGetValue(Options.NewTemplate.Long, out var _);
        var template = commandLineArgs.Options.GetOrNull(Options.Template.Short, Options.Template.Long);
        var newProTemplate = !string.IsNullOrEmpty(template) && template == ModuleProTemplate.TemplateName;
        var skipDbMigrations = newTemplate || newProTemplate || commandLineArgs.Options.TryGetValue(Options.DbMigrations.Skip, out var _);
        var solutionFile = GetSolutionFile(commandLineArgs);

        var version = commandLineArgs.Options.GetOrNull(Options.Version.Short, Options.Version.Long);
        version ??= SolutionPackageVersionFinder.FindByCsprojVersion(solutionFile, "X.Abp");

        // $"{CliUrls.WwwAbpIo}api/app/module/byNameWithDetails/?name=
        var moduleInfo = await SolutionModuleAdder.AddAsync(
             solutionFile,
             commandLineArgs.Target,
             version,
             skipDbMigrations,
             false,
             false,
             newTemplate,
             newProTemplate);

        _lastAddedModuleInfo = new AddModuleInfoOutput
        {
            DisplayName = moduleInfo.DisplayName,
            Name = moduleInfo.Name,
            DocumentationLinks = moduleInfo.DocumentationLinks,
            InstallationCompleteMessage = moduleInfo.InstallationCompleteMessage
        };
    }

    public string GetUsageInfo()
    {
        var sb = new StringBuilder();

        sb.AppendLine(string.Empty);
        sb.AppendLine("'install-module' command is used to add a multi-package ABP module to a solution.");
        sb.AppendLine("It should be used in a folder containing a .sln file.");
        sb.AppendLine(string.Empty);
        sb.AppendLine("Usage:");
        sb.AppendLine("  xabp install-module <module-name> [options]");
        sb.AppendLine(string.Empty);
        sb.AppendLine("Options:");
        sb.AppendLine("  --new                                           Creates a fresh new module (specialized for your solution) and adds it your solution.");
        sb.AppendLine("  -s|--solution <solution-file>                   Specify the solution file explicitly.");
        sb.AppendLine("  --skip-db-migrations <boolean>                  Specify if a new migration will be added or not.  (Always True if `--new` is used.)");
        sb.AppendLine("  -sp|--startup-project <startup-project-path>    Relative path to the project folder of the startup project. Default value is the current folder.");
        sb.AppendLine("  -v|--version <version>                          Specify the version of the module. Default is your project's ABP version.");
        sb.AppendLine(string.Empty);
        sb.AppendLine("Examples:");
        sb.AppendLine(string.Empty);
        sb.AppendLine("  xabp install-module X.Blogging                      Adds the module to the current solution.");
        sb.AppendLine("  xabp install-module X.Blogging -s Acme.BookStore    Adds the module to the given solution.");
        sb.AppendLine("  xabp install-module X.Blogging -s Acme.BookStore --skip-db-migrations false    Adds the module to the given solution but doesn't create a database migration.");
        sb.AppendLine(@"  xabp install-module X.Blogging -s Acme.BookStore -sp ..\Acme.BookStore.Web\Acme.BookStore.Web.csproj   Adds the module to the given solution and specify migration startup project.");
        sb.AppendLine(@"  xabp install-module ProductManagement --new -sp ..\Acme.BookStore.Web\Acme.BookStore.Web.csproj   Crates a new module named `ProductManagement` and adds it to your solution.");
        sb.AppendLine(string.Empty);
        sb.AppendLine("See the documentation for more info: https://docs.abp.io/en/abp/latest/CLI");

        return sb.ToString();
    }

    public string GetShortDescription()
    {
        return "Add a multi-package module to a solution by finding all packages of the module, " +
               "finding related projects in the solution and adding each package to the corresponding project in the solution.";
    }

    protected virtual string GetSolutionFile(CommandLineArgs commandLineArgs)
    {
        var providedSolutionFile = PathHelper.NormalizePath(
            commandLineArgs.Options.GetOrNull(
                Options.Solution.Short,
                Options.Solution.Long));

        if (!providedSolutionFile.IsNullOrWhiteSpace())
        {
            return providedSolutionFile;
        }

        var foundSolutionFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.sln");
        if (foundSolutionFiles.Length == 1)
        {
            return foundSolutionFiles[0];
        }

        if (foundSolutionFiles.Length == 0)
        {
            throw new CliUsageException("'xabp install-module' command should be used inside a folder containing a .sln file!");
        }

        // foundSolutionFiles.Length > 1
        var sb = new StringBuilder("There are multiple solution (.sln) files in the current directory. Please specify one of the files below:");

        foreach (var foundSolutionFile in foundSolutionFiles)
        {
            sb.AppendLine("* " + foundSolutionFile);
        }

        sb.AppendLine("Example:");
        sb.AppendLine($"xabp install-module {commandLineArgs.Target} -p {foundSolutionFiles[0]}");

        throw new CliUsageException(sb.ToString());
    }

    public static class Options
    {
        public static class Solution
        {
            public const string Short = "s";
            public const string Long = "solution";
        }

        public static class Version
        {
            public const string Short = "v";
            public const string Long = "version";
        }

        public static class DbMigrations
        {
            public const string Skip = "skip-db-migrations";
        }

        public class NewTemplate
        {
            public const string Long = "new";
        }

        public class Template
        {
            public const string Short = "t";
            public const string Long = "template";
        }
    }
}
