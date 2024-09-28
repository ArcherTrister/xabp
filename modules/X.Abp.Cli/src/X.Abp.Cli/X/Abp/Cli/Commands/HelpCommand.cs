// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

using Volo.Abp.Cli;
using Volo.Abp.Cli.Args;
using Volo.Abp.Cli.Commands;
using Volo.Abp.DependencyInjection;

namespace X.Abp.Cli.Commands;

public class HelpCommand : IConsoleCommand, ITransientDependency
{
#pragma warning disable SA1122 // Use string.Empty for empty strings
#pragma warning disable CA1834 // 若适用，请考虑使用 "StringBuilder.Append(char)"
    public const string Name = "help";

    protected static string CommandName => Name;

    protected ILogger<HelpCommand> Logger { get; set; }

    protected AbpCliOptions AbpCliOptions { get; }

    protected IServiceScopeFactory ServiceScopeFactory { get; }

    public HelpCommand(IOptions<AbpCliOptions> cliOptions,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<HelpCommand> logger)
    {
        AbpCliOptions = cliOptions.Value;
        ServiceScopeFactory = serviceScopeFactory;
        Logger = logger ?? NullLogger<HelpCommand>.Instance;
    }

    public virtual Task ExecuteAsync(CommandLineArgs commandLineArgs)
    {
        if (string.IsNullOrWhiteSpace(commandLineArgs.Target))
        {
            Logger.LogInformation(GetUsageInfo());
            return Task.CompletedTask;
        }

        if (!AbpCliOptions.Commands.TryGetValue(commandLineArgs.Target, out var _))
        {
            Logger.LogWarning("There is no command named {Target}.", commandLineArgs.Target);
            Logger.LogInformation(GetUsageInfo());
            return Task.CompletedTask;
        }

        var commandType = AbpCliOptions.Commands[commandLineArgs.Target];

        using (var scope = ServiceScopeFactory.CreateScope())
        {
            var command = (IConsoleCommand)scope.ServiceProvider.GetRequiredService(commandType);
            Logger.LogInformation(command.GetUsageInfo());
        }

        return Task.CompletedTask;
    }

    public string GetUsageInfo()
    {
        var sb = new StringBuilder();

        sb.AppendLine(string.Empty);
        sb.AppendLine("Usage:");
        sb.AppendLine(string.Empty);
        sb.AppendLine("    xabp <command> <target> [options]");
        sb.AppendLine(string.Empty);
        sb.AppendLine("Command List:");
        sb.AppendLine(string.Empty);

        foreach (var command in AbpCliOptions.Commands.ToArray())
        {
            var method = command.Value.GetMethod("GetShortDescription", BindingFlags.Static | BindingFlags.Public);
            if (method == null)
            {
                continue;
            }

            var shortDescription = (string)method.Invoke(null, null);

            sb.Append("    > ");
            sb.Append(command.Key);
            sb.Append(string.IsNullOrWhiteSpace(shortDescription) ? "" : ":");
            sb.Append(" ");
            sb.AppendLine(shortDescription);
        }

        sb.AppendLine(string.Empty);
        sb.AppendLine("To get a detailed help for a command:");
        sb.AppendLine(string.Empty);
        sb.AppendLine("    xabp help <command>");
        sb.AppendLine(string.Empty);

        return sb.ToString();
    }

    public static string GetShortDescription()
    {
        return "Show command line help. Write ` xabp help <command> `";
    }
}
