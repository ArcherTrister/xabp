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
using Volo.Abp.DependencyInjection;
using Volo.Abp.IO;

using X.Abp.Cli.CertGenerating;

namespace X.Abp.Cli.Commands;

public class GenerateCertCommand : IConsoleCommand, ITransientDependency
{
    public const string Name = "generate-cert";

    protected static string CommandName => Name;

    protected ILogger<GenerateCertCommand> Logger { get; set; }

    protected AbpCliOptions AbpCliOptions { get; }

    // protected IServiceScopeFactory ServiceScopeFactory { get; }
    protected ICertGenerator CertGenerator { get; }

    public GenerateCertCommand(
        IOptions<AbpCliOptions> cliOptions,
        ICertGenerator certGenerator,
        ILogger<GenerateCertCommand> logger)
    {
        AbpCliOptions = cliOptions.Value;
        CertGenerator = certGenerator;

        // ServiceScopeFactory = serviceScopeFactory;
        Logger = logger ?? NullLogger<GenerateCertCommand>.Instance;
    }

    public async Task ExecuteAsync(CommandLineArgs commandLineArgs)
    {
        await CertGenerator.GenerateCertAsync(GetCertGenerateArgs(commandLineArgs));
    }

    protected CertGenerateArgs GetCertGenerateArgs(CommandLineArgs commandLineArgs)
    {
        var workDirectory = commandLineArgs.Options.GetOrNull(Options.WorkDirectory.ShortArgName, Options.WorkDirectory.LongArgName) ?? Directory.GetCurrentDirectory();

        var outputFolder = commandLineArgs.Options.GetOrNull(Options.OutputFolder.ShortArgName, Options.OutputFolder.LongArgName);

        var outputFolderRoot =
            !string.IsNullOrWhiteSpace(outputFolder) ? Path.Combine(workDirectory, outputFolder) : workDirectory;
        if (outputFolderRoot != null)
        {
            Logger.LogInformation("Output folder: " + outputFolderRoot);
        }

        DirectoryHelper.CreateIfNotExists(outputFolderRoot);

        var certName = commandLineArgs.Options.GetOrNull(Options.CertName.ShortArgName, Options.CertName.LongArgName) ?? "idsrv4";

        Logger.LogInformation("Cert Name: " + certName);

        var certType = commandLineArgs.Options.GetOrNull(Options.CertType.ShortArgName, Options.CertType.LongArgName) ?? "rsa";

        Logger.LogInformation("Cert Type: " + certType);

        var password = commandLineArgs.Options.GetOrNull(Options.Password.ShortArgName, Options.Password.LongArgName) ?? "123456";

        Logger.LogInformation("Password: " + password);

        var dnsName = commandLineArgs.Options.GetOrNull(Options.DnsName.ShortArgName, Options.DnsName.LongArgName) ?? "localhost";

        Logger.LogInformation("DnsName: " + dnsName);

        var validityPeriodInYears = 1;
        var years = commandLineArgs.Options.GetOrNull(Options.Years.ShortArgName, Options.Years.LongArgName);

        if (years != null)
        {
            Logger.LogInformation("Validity Period In Years: " + years);
            validityPeriodInYears = Convert.ToInt32(years);
        }

        return new CertGenerateArgs(
            outputFolderRoot, certName, certType, password, dnsName, validityPeriodInYears);
    }

    public string GetUsageInfo()
    {
        var sb = new StringBuilder();

        sb.AppendLine(string.Empty);
        sb.AppendLine("Usage:");
        sb.AppendLine(string.Empty);
        sb.AppendLine($"  xabp {CommandName}");
        sb.AppendLine(string.Empty);
        sb.AppendLine("Options:");
        sb.AppendLine(string.Empty);

        sb.AppendLine("-wd|--working-directory <directory-path>          Execution directory.");

        sb.AppendLine("-t|--type <generate-cert-type>                         The name of generate type (rsa, ecd).");
        sb.AppendLine("  rsa");
        sb.AppendLine("    -o|--output <folder-name>                            (default: '') Folder name to place generated Cert File in.");
        sb.AppendLine("    -p|--password <password>                            (default: '123456') The cert file password.");
        sb.AppendLine("    -d|--dns-name <dns-name>                            (default: 'localhost') The cert file domain name.");
        sb.AppendLine("    -y|--years <years>                            (default: '1') The cert file valid duration.");
        sb.AppendLine("  ecd");
        sb.AppendLine("    -o|--output <folder-name>                            (default: '') Folder name to place generated Cert File in.");
        sb.AppendLine("    -p|--password <password>                            (default: '123456') The cert file password.");
        sb.AppendLine("    -d|--dns-name <dns-name>                            (default: 'localhost') The cert file domain name.");
        sb.AppendLine("    -y|--years <years>                            (default: '1') The cert file valid duration.");
        sb.AppendLine("Examples:");
        sb.AppendLine(string.Empty);
        sb.AppendLine("  xabp generate-cert");
        sb.AppendLine("  xabp generate-cert -o MyCerts/InnerFolder");
        sb.AppendLine("  xabp generate-cert -o MyCerts/InnerFolder -p 123456");
        sb.AppendLine("  xabp generate-cert -t rsa -o MyCerts/InnerFolder -p 123456");
        sb.AppendLine("  xabp generate-cert -t ecd -o MyCerts/InnerFolder -p 123456");
        sb.AppendLine("  xabp generate-cert -t ecd -o MyCerts/InnerFolder -p 123456 -y 10");
        sb.AppendLine("  xabp generate-cert -t ecd -o MyCerts/InnerFolder -p 123456 -d www.domain.com -y 10");
        sb.AppendLine(string.Empty);

        // sb.AppendLine("See the documentation for more info: https://docs.abp.io/en/abp/latest/CLI");
        return sb.ToString();
    }

    public string GetShortDescription()
    {
        return "Generates cert file.";
    }

    public static class Options
    {
        public static class WorkDirectory
        {
            public const string ShortArgName = "wd";
            public const string LongArgName = "working-directory";
        }

        public static class CertType
        {
            public const string ShortArgName = "t";
            public const string LongArgName = "type";
        }

        public static class OutputFolder
        {
            public const string ShortArgName = "o";
            public const string LongArgName = "output";
        }

        public static class CertName
        {
            public const string ShortArgName = "n";
            public const string LongArgName = "name";
        }

        public static class Password
        {
            public const string ShortArgName = "p";
            public const string LongArgName = "password";
        }

        public static class DnsName
        {
            public const string ShortArgName = "d";
            public const string LongArgName = "dns-name";
        }

        public static class Years
        {
            public const string ShortArgName = "y";
            public const string LongArgName = "years";
        }
    }
}
