// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.Autofac;
using Volo.Abp.Cli;
using Volo.Abp.Cli.Commands;
using Volo.Abp.Modularity;

namespace X.Abp.Cli;

[DependsOn(
    typeof(AbpCliCoreModule),
    typeof(AbpAutofacModule))]
public class AbpCliModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddCertificateManager();

        Configure<AbpCliOptions>(options =>
        {
            options.Commands.Clear();

            // options.Commands[HelpCommand.Name] = typeof(HelpCommand);
            options.Commands[Commands.HelpCommand.Name] = typeof(Commands.HelpCommand);
            options.Commands[PromptCommand.Name] = typeof(PromptCommand);
            options.Commands[CliCommand.Name] = typeof(CliCommand);
            options.Commands[NewCommand.Name] = typeof(NewCommand);
            options.Commands[Commands.CreateCommand.Name] = typeof(Commands.CreateCommand);

            options.Commands[GetSourceCommand.Name] = typeof(GetSourceCommand);
            options.Commands[UpdateCommand.Name] = typeof(UpdateCommand);

            options.Commands[AddPackageCommand.Name] = typeof(AddPackageCommand);

            // options.Commands[ListModulesCommand.Name] = typeof(ListModulesCommand);
            options.Commands[Commands.ListModulesCommand.Name] = typeof(Commands.ListModulesCommand);
            options.Commands[AddModuleCommand.Name] = typeof(AddModuleCommand);
            options.Commands[Commands.InstallModuleCommand.Name] = typeof(Commands.InstallModuleCommand);

            options.Commands[ListTemplatesCommand.Name] = typeof(ListTemplatesCommand);
            options.Commands[LoginCommand.Name] = typeof(LoginCommand);
            options.Commands[LoginInfoCommand.Name] = typeof(LoginInfoCommand);
            options.Commands[LogoutCommand.Name] = typeof(LogoutCommand);

            options.Commands[SuiteCommand.Name] = typeof(SuiteCommand);
            options.Commands[SwitchToPreviewCommand.Name] = typeof(SwitchToPreviewCommand);
            options.Commands[SwitchToStableCommand.Name] = typeof(SwitchToStableCommand);
            options.Commands[SwitchToNightlyCommand.Name] = typeof(SwitchToNightlyCommand);
            options.Commands[TranslateCommand.Name] = typeof(TranslateCommand);

            options.Commands[BuildCommand.Name] = typeof(BuildCommand);
            options.Commands[BundleCommand.Name] = typeof(BundleCommand);
            options.Commands[CreateMigrationAndRunMigratorCommand.Name] = typeof(CreateMigrationAndRunMigratorCommand);
            options.Commands[InstallLibsCommand.Name] = typeof(InstallLibsCommand);
            options.Commands[CleanCommand.Name] = typeof(CleanCommand);

            options.Commands[GenerateProxyCommand.Name] = typeof(GenerateProxyCommand);

            options.Commands[RemoveProxyCommand.Name] = typeof(RemoveProxyCommand);

            options.Commands[Commands.GenerateCertCommand.Name] = typeof(Commands.GenerateCertCommand);
            options.Commands[Commands.ShowVersionCommand.Name] = typeof(Commands.ShowVersionCommand);
        });
    }
}
