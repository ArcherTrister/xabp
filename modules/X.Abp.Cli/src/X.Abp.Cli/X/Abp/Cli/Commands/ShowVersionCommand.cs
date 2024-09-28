// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Volo.Abp.Cli.Args;
using Volo.Abp.Cli.Commands;
using Volo.Abp.DependencyInjection;

namespace X.Abp.Cli.Commands;

public class ShowVersionCommand : IConsoleCommand, ITransientDependency
{
  public const string Name = "show-version";

  protected ILogger<ShowVersionCommand> Logger { get; set; }

  public ShowVersionCommand(ILogger<ShowVersionCommand> logger)
  {
    Logger = logger ?? NullLogger<ShowVersionCommand>.Instance;
  }

  public virtual Task ExecuteAsync(CommandLineArgs commandLineArgs)
  {
    Logger.LogInformation("xabp version is 7.2.3");
    return Task.CompletedTask;
  }

  public string GetUsageInfo()
  {
    var sb = new StringBuilder();

    sb.AppendLine(string.Empty);
    sb.AppendLine("'show-version' command is used for version.");
    sb.AppendLine(string.Empty);
    sb.AppendLine("Usage:");
    sb.AppendLine("  xabp show-version");

    return sb.ToString();
  }

  public static string GetShortDescription()
  {
    return "Show xabp version";
  }
}
