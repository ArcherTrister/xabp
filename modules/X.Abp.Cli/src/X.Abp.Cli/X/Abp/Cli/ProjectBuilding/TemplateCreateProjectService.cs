// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Newtonsoft.Json.Linq;

using Volo.Abp;
using Volo.Abp.Cli;
using Volo.Abp.Cli.Commands.Services;
using Volo.Abp.Cli.ProjectBuilding;
using Volo.Abp.Cli.ProjectBuilding.Templates.App;
using Volo.Abp.Cli.Utils;
using Volo.Abp.DependencyInjection;
using Volo.Abp.IO;

using X.Abp.Cli.ProjectBuilding.Templates;

namespace X.Abp.Cli.ProjectBuilding;

public class TemplateCreateProjectService : ICreateProjectService, ISingletonDependency
{
    protected const string DefaultPassPhrase = "gsKnGZ041HLL4IM8";

    protected const string DefaultVersion = "8.3.2";

    private static readonly string[] ExclusionFoldersOrFiles =
    {
        ".github", ".vs", ".svn",
        ".dockerignore", ".gitattributes",
        ".prettierrc", "LICENSE", ".bat",
        ".exe", ".dll", ".bin",
        ".suo", ".obj", ".pdb",
        ".png", ".jpg", ".jpeg",
        ".ico", ".woff", ".woff2",
        ".eot", ".svg", ".ttf",
        ".pfx", ".jwk"
    };

    private static readonly string[] SourceTexts =
    {
        "mycompanyname", "myprojectname"
    };

    private static class InstallTemplate
    {
        public const string ShortArgName = "it";
        public const string LongArgName = "install-template";
    }

    private static class TemplateType
    {
        public const string ShortArgName = "tt";
        public const string LongArgName = "template-type";
    }

    protected InitialMigrationCreator InitialMigrationCreator { get; }

    protected ICmdHelper CmdHelper { get; }

    protected ILogger<TemplateCreateProjectService> Logger { get; set; }

    public TemplateCreateProjectService(InitialMigrationCreator initialMigrationCreator, ICmdHelper cmdHelper, ILogger<TemplateCreateProjectService> logger)
    {
        InitialMigrationCreator = initialMigrationCreator;
        CmdHelper = cmdHelper;

        Logger = logger ?? NullLogger<TemplateCreateProjectService>.Instance;
    }

    public virtual async Task CreateAsync(ProjectBuildArgs createArgs)
    {
        Logger.LogInformation("Execute dotnet command...");

        if (!createArgs.ExtraProperties.TryGetValue(InstallTemplate.ShortArgName, out var installTemplate))
        {
            if (!createArgs.ExtraProperties.TryGetValue(InstallTemplate.LongArgName, out installTemplate))
            {
                installTemplate = "false";
            }
        }

        if (installTemplate.Equals("true", StringComparison.OrdinalIgnoreCase))
        {
            var installCommand = $"dotnet new install X.Abp.Templates::{(!string.IsNullOrWhiteSpace(createArgs.Version) ? $"{createArgs.Version}" : DefaultVersion)} --force";

            Logger.LogInformation("Execute command: " + installCommand);

            var installError = CmdHelper.RunCmdAndGetOutput(installCommand, out bool installIsSuccessful);
            if (!installIsSuccessful)
            {
                Logger.LogError("Template install fail: " + installError);
                return;
            }

            Logger.LogInformation("Template install success: " + installError);
        }

        var commandBuilder = new StringBuilder("dotnet new");
        commandBuilder.AppendFormat(" {0}", createArgs.TemplateName);
        if (AbpTemplateConsts.NewTemplates.Any(p => p == createArgs.TemplateName))
        {
            commandBuilder.AppendFormat(" -tt {0}", GetTemplateType(createArgs));
        }

        commandBuilder.AppendFormat(" -n {0}", createArgs.SolutionName.FullName);
        commandBuilder.AppendFormat(" -o {0}", createArgs.OutputFolder);
        commandBuilder.AppendFormat(" -dbms {0}", createArgs.DatabaseManagementSystem.ToString());

        Logger.LogInformation("Execute command: " + commandBuilder.ToString());

        var cmdError = CmdHelper.RunCmdAndGetOutput(commandBuilder.ToString(), out bool isSuccessful);
        if (!isSuccessful)
        {
            Logger.LogError("Execute command error: " + cmdError);
            return;
        }

        Logger.LogInformation("Executed command success: " + cmdError);

        Logger.LogInformation("Search Solution files.");
        var projectFiles = new List<ProjectFile>();
        SearchSolutionPath(projectFiles, createArgs.OutputFolder, 0);

        // await TryReplaceXAbpVersionAsync(projectFiles, createArgs.Version);
        await TryReplaceAppSettingsAsync(projectFiles, createArgs.ConnectionString);

        await TryReplaceFileWithProjectFileAsync(
            projectFiles,
            createArgs.SolutionName.ProjectName);

        TryReplaceFileNameWithProjectFolder(
            projectFiles,
            createArgs.SolutionName.ProjectName);

        // TryRunMigrator(projectFiles);
        await CreateInitialMigrationsAsync(createArgs);

        Logger.LogInformation("'{ProjectName}' has been successfully created to '{OutputFolder}'", createArgs.SolutionName.ProjectName, createArgs.OutputFolder);
    }

    protected virtual string GetTemplateType(ProjectBuildArgs createArgs)
    {
        if (!createArgs.ExtraProperties.TryGetValue(TemplateType.ShortArgName, out var templateType))
        {
            if (!createArgs.ExtraProperties.TryGetValue(TemplateType.LongArgName, out templateType))
            {
                templateType = "IdentityServer4";
            }
        }

        if (templateType == null)
        {
            return "IdentityServer4";
        }

        return templateType.ToLowerInvariant() switch
        {
            "identityserver4" => "IdentityServer4",
            "openiddict" => "OpenIddict",
            _ => throw new CliUsageException(ExceptionMessageHelper.GetInvalidOptionExceptionMessage("Template Type")),
        };
    }

    protected virtual void SearchSolutionPath(List<ProjectFile> projectFiles, string solutionPath, int depth)
    {
        var searchFiles = Directory.GetFileSystemEntries(solutionPath, "*.*", SearchOption.TopDirectoryOnly);
        searchFiles = searchFiles.Where(f => !ExclusionFoldersOrFiles.Any(ef => f.EndsWith(ef, StringComparison.OrdinalIgnoreCase))).ToArray();
        foreach (var searchFile in searchFiles)
        {
            projectFiles.Add(new ProjectFile(solutionPath, searchFile, depth, Directory.Exists(searchFile)));
            if (Directory.Exists(searchFile))
            {
                SearchSolutionPath(projectFiles, searchFile, depth++);
            }
        }
    }

    protected virtual async Task TryReplaceXAbpVersionAsync(List<ProjectFile> projectFiles, string versionArg)
    {
        if (string.IsNullOrWhiteSpace(versionArg))
        {
            return;
        }

        Logger.LogInformation("Replace XAbpVersion.");

        var buildPropsFile = projectFiles.Where(f =>
            f.Name.EndsWith("Directory.Build.props", StringComparison.Ordinal))
            .FirstOrDefault();

        if (buildPropsFile != null)
        {
            await ReplaceFileTextAsync(buildPropsFile, $"<XAbpPackageVersion>{DefaultVersion}</XAbpPackageVersion>", $"<XAbpPackageVersion>{versionArg}</XAbpPackageVersion>");
        }
    }

    protected virtual async Task TryReplaceAppSettingsAsync(List<ProjectFile> projectFiles, string connectionStringArg)
    {
        if (string.IsNullOrWhiteSpace(connectionStringArg))
        {
            return;
        }

        const string DefaultConnectionStringKey = "Default";

        var appSettingsJsonFiles = projectFiles.Where(f =>
            f.Name.EndsWith("appsettings.json", StringComparison.Ordinal))
            .ToArray();

        if (appSettingsJsonFiles.Length == 0)
        {
            return;
        }

        Logger.LogInformation("Replace AppSettings.");

        var newConnectionString = $"\"{DefaultConnectionStringKey}\": \"{connectionStringArg.Replace(@"\\", @"\", StringComparison.Ordinal).Replace(@"\", @"\\", StringComparison.Ordinal)}\"";
        var randomPassPhrase = GetRandomPassPhrase();

        foreach (var appSettingsJson in appSettingsJsonFiles)
        {
            try
            {
                string appSettingJsonContentWithoutBom;
                using (var stream = new StreamReader(appSettingsJson.Name, Encoding.UTF8))
                {
                    appSettingJsonContentWithoutBom = await stream.ReadToEndAsync();
                }

                var jsonObject = JObject.Parse(appSettingJsonContentWithoutBom);

                var connectionStringContainer = (JContainer)jsonObject?["ConnectionStrings"];
                if (connectionStringContainer == null)
                {
                    continue;
                }

                if (connectionStringContainer.Count == 0)
                {
                    continue;
                }

                var connectionStrings = connectionStringContainer.ToList();

                foreach (var connectionString in connectionStrings)
                {
                    var property = (JProperty)connectionString;
                    var connectionStringName = property.Name;

                    if (connectionStringName == DefaultConnectionStringKey)
                    {
                        var defaultConnectionString = property.ToString();
                        if (defaultConnectionString == null)
                        {
                            continue;
                        }

                        appSettingJsonContentWithoutBom = appSettingJsonContentWithoutBom.Replace(defaultConnectionString, newConnectionString, StringComparison.Ordinal);
                        break;
                    }
                }

                appSettingJsonContentWithoutBom = appSettingJsonContentWithoutBom.Replace(DefaultPassPhrase, randomPassPhrase, StringComparison.Ordinal);

                using var writer = new StreamWriter(appSettingsJson.Name, false, Encoding.UTF8);
                await writer.WriteAsync(appSettingJsonContentWithoutBom);
            }
            catch (Exception ex)
            {
                Logger.LogError("Cannot change the connection string in " + appSettingsJson.Name + ". Error: " + ex.Message);
            }
        }
    }

    protected virtual async Task TryReplaceFileWithProjectFileAsync(List<ProjectFile> projectFiles, string projectName)
    {
        Logger.LogInformation("Replace FileContent.");

        var canReplaceFiles = projectFiles.Where(f => !f.IsFolder);
        foreach (var projectFile in canReplaceFiles)
        {
            await ReplaceFileTextAsync(projectFile, "MyProjectName", projectName);
        }
    }

    protected virtual void TryReplaceFileNameWithProjectFolder(List<ProjectFile> projectFiles, string projectName)
    {
        Logger.LogInformation("Replace FileName.");

        var canReplaceFiles = projectFiles
            .OrderByDescending(f => f.Depth)
            .OrderByDescending(f => !f.IsFolder);
        foreach (var projectFile in canReplaceFiles)
        {
            var replaceFileName = projectFile.Name.Replace("MyProjectName", projectName, StringComparison.Ordinal);

            if (File.Exists(projectFile.Name))
            {
                DirectoryHelper.CreateIfNotExists(Path.GetDirectoryName(replaceFileName));
                File.Move(projectFile.Name, replaceFileName);
            }
        }

        var canReplacePaths = projectFiles
            .Where(projectFile => projectFile.Name.Contains("MyProjectName", StringComparison.Ordinal))
            .OrderByDescending(f => f.Depth)
            .OrderByDescending(f => f.IsFolder);
        foreach (var projectFile in canReplacePaths)
        {
            DirectoryHelper.DeleteIfExists(projectFile.Name, true);
        }
    }

    protected virtual void TryRunMigrator(List<ProjectFile> projectFiles)
    {
        try
        {
            var dbMigratorProject = projectFiles.Where(p => !p.IsFolder && p.Name.EndsWith(".DbMigrator.csproj", StringComparison.Ordinal)).Select(p => p.Name).FirstOrDefault();

            if (!string.IsNullOrEmpty(dbMigratorProject))
            {
                Logger.LogInformation("Run Migrator.");

                CmdHelper.RunCmd($"dotnet run", out var exitCode, workingDirectory: Path.GetDirectoryName(dbMigratorProject));
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Run Migrator An error occurred");
        }
    }

    protected async Task CreateInitialMigrationsAsync(ProjectBuildArgs projectArgs)
    {
        bool isLayeredTemplate;
        string efCoreProjectPath;
        switch (projectArgs.TemplateName)
        {
            // case AbpTemplateConsts.MicroService:
            case AbpTemplateConsts.MultiLayer:
            case AbpTemplateConsts.MultiLayerSeparateAuthServer:
            case AbpTemplateConsts.MultiLayerSeparatedTenantSchema:
            case AbpTemplateConsts.IdentityServer.Default:
            case AbpTemplateConsts.IdentityServer.SeparatedAuthServer:
            case AbpTemplateConsts.IdentityServer.SeparatedTenantSchema:
            case AbpTemplateConsts.IdentityServer.SeparatedAuthServerSeparatedTenantSchema:
            case AbpTemplateConsts.Openiddict.Default:
            case AbpTemplateConsts.Openiddict.SeparatedAuthServer:
            case AbpTemplateConsts.Openiddict.SeparatedTenantSchema:
            case AbpTemplateConsts.Openiddict.SeparatedAuthServerSeparatedTenantSchema:
                efCoreProjectPath = Directory.GetFiles(projectArgs.OutputFolder, "*EntityFrameworkCore.csproj", SearchOption.AllDirectories).FirstOrDefault();
                isLayeredTemplate = true;
                break;
            case AbpTemplateConsts.SingleLayer:
            case AppNoLayersTemplate.TemplateName:
            case AppNoLayersProTemplate.TemplateName:
                efCoreProjectPath = Directory.GetFiles(projectArgs.OutputFolder, "*.Host.csproj", SearchOption.AllDirectories).FirstOrDefault()
                    ?? Directory.GetFiles(projectArgs.OutputFolder, "*.csproj", SearchOption.AllDirectories).FirstOrDefault();
                isLayeredTemplate = false;
                break;
            default:
                return;
        }

        if (string.IsNullOrWhiteSpace(efCoreProjectPath))
        {
            Logger.LogWarning("Couldn't find the project to create initial migrations!");
            return;
        }

        await InitialMigrationCreator.CreateAsync(Path.GetDirectoryName(efCoreProjectPath), isLayeredTemplate);
    }

    protected virtual string GetRandomPassPhrase()
    {
        const string letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var builder = new StringBuilder();
        for (var i = 0; i < DefaultPassPhrase.Length; i++)
        {
            builder.Append(letters[RandomHelper.GetRandom(0, letters.Length)]);
        }

        return builder.ToString();
    }

    protected static async Task ReplaceFileTextAsync(ProjectFile projectFile, string sourceText, string replaceText)
    {
        string fileText;
        using (var stream = new StreamReader(projectFile.Name, Encoding.UTF8))
        {
            fileText = await stream.ReadToEndAsync();
        }

        fileText = fileText.Replace(sourceText, replaceText, StringComparison.Ordinal);
        fileText = fileText.Replace(sourceText.ToLower(System.Globalization.CultureInfo.CurrentCulture), replaceText.ToLower(System.Globalization.CultureInfo.CurrentCulture), StringComparison.Ordinal);

        using var writer = new StreamWriter(projectFile.Name, false, Encoding.UTF8);
        await writer.WriteAsync(fileText);
    }

    protected static async Task ReplaceFileTextAsync(ProjectFile projectFile, string[] replaceTexts)
    {
        string fileText;
        using (var stream = new StreamReader(projectFile.Name, Encoding.UTF8))
        {
            fileText = await stream.ReadToEndAsync();
        }

        for (var i = 0; i < SourceTexts.Length; i++)
        {
            fileText = fileText.Replace(SourceTexts[i], replaceTexts[i], StringComparison.Ordinal);
        }

        using var writer = new StreamWriter(projectFile.Name, false, Encoding.UTF8);
        await writer.WriteAsync(fileText);
    }
}
