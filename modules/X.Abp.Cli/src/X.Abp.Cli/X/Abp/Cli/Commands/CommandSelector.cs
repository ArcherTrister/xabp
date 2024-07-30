// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;

using Microsoft.Extensions.Options;

using Volo.Abp.Cli;
using Volo.Abp.Cli.Args;
using Volo.Abp.Cli.Commands;
using Volo.Abp.DependencyInjection;

namespace X.Abp.Cli.Commands;

[Dependency(ReplaceServices = true)]
public class CommandSelector : ICommandSelector, ITransientDependency
{
    protected AbpCliOptions Options { get; }

    public CommandSelector(IOptions<AbpCliOptions> options)
    {
        Options = options.Value;
    }

    public Type Select(CommandLineArgs commandLineArgs)
    {
        return commandLineArgs.Command.IsNullOrWhiteSpace()
            ? typeof(HelpCommand)
            : Options.Commands.GetOrDefault(commandLineArgs.Command)
               ?? typeof(HelpCommand);
    }
}
