// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Volo.Abp.Cli.Args;
using Volo.Abp.Cli.Commands;
using Volo.Abp.Cli.ProjectBuilding;
using Volo.Abp.DependencyInjection;

using X.Abp.Cli.ProjectModification;

namespace X.Abp.Cli.Commands;

public class ListModulesCommand : IConsoleCommand, ITransientDependency
{
  public const string Name = "list-modules";

  protected ModuleInfoProvider ModuleInfoProvider { get; }

  protected ILogger<ListModulesCommand> Logger { get; set; }

  public ListModulesCommand(ModuleInfoProvider moduleInfoProvider, ILogger<ListModulesCommand> logger)
  {
    ModuleInfoProvider = moduleInfoProvider;
    Logger = logger ?? NullLogger<ListModulesCommand>.Instance;
  }

  public virtual async Task ExecuteAsync(CommandLineArgs commandLineArgs)
  {
    var modules = await ModuleInfoProvider.GetModuleListAsync();
    var freeModules = modules.Where(m => !m.IsPro).ToList();

    var proModules = modules.Where(m => m.IsPro).ToList();
    var xModules = InstallModules.Init.Select(p => new Volo.Abp.Cli.ProjectBuilding.Building.ModuleInfo
    {
      Name = p.Name,
      DisplayName = p.DisplayName,
    }).ToList();
    proModules = proModules.Union(xModules).ToList();

    var output = new StringBuilder(Environment.NewLine);
    output.AppendLine("Open Source Application Modules");
    output.AppendLine();

    foreach (var module in freeModules)
    {
      output.AppendLine($"> {module.DisplayName.PadRight(50)} ({module.Name})");
    }

    if (commandLineArgs.Options.TryGetValue("include-pro-modules", out var _))
    {
      output.AppendLine();
      output.AppendLine("Commercial (Pro) Application Modules");
      output.AppendLine();
      foreach (var module in proModules)
      {
        output.AppendLine($"> {module.DisplayName.PadRight(50)} ({module.Name})");
      }
    }

    Logger.LogInformation(output.ToString());
  }

  public string GetUsageInfo()
  {
    var sb = new StringBuilder();

    sb.AppendLine(string.Empty);
    sb.AppendLine("'list-modules' command is used for listing open source application modules.");
    sb.AppendLine(string.Empty);
    sb.AppendLine("Usage:");
    sb.AppendLine("  xabp list-modules");
    sb.AppendLine("  xabp list-modules --include-pro-modules");
    sb.AppendLine(string.Empty);
    sb.AppendLine("Options:");
    sb.AppendLine("  --include-pro-modules                                           Includes commercial (pro) modules in the output.");
    sb.AppendLine(string.Empty);
    sb.AppendLine("See the documentation for more info: https://docs.abp.io/en/abp/latest/CLI");

    return sb.ToString();
  }

  public static string GetShortDescription()
  {
    return "List application modules";
  }
}
