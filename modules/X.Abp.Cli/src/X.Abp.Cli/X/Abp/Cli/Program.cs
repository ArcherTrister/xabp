// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

using Volo.Abp;
using Volo.Abp.Cli;

namespace X.Abp.Cli;

public class Program
{
    // TODO: GetXmlDocsSummary
    private static async Task Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var loggerOutputTemplate = "{Message:lj}{NewLine}{Exception}";
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Volo.Abp", LogEventLevel.Warning)
            .MinimumLevel.Override("System.Net.Http.HttpClient", LogEventLevel.Warning)
            .MinimumLevel.Override("Volo.Abp.IdentityModel", LogEventLevel.Information)
            .MinimumLevel.Override("X.Abp", LogEventLevel.Warning)
            .MinimumLevel.Override("System.Net.Http.HttpClient", LogEventLevel.Warning)
#if DEBUG
            .MinimumLevel.Override("Volo.Abp.Cli", LogEventLevel.Debug)
            .MinimumLevel.Override("X.Abp.Cli", LogEventLevel.Debug)
#else
        .MinimumLevel.Override("Volo.Abp.Cli", LogEventLevel.Information)
        .MinimumLevel.Override("X.Abp.Cli", LogEventLevel.Information)
#endif
            .Enrich.FromLogContext()
            .WriteTo.File(Path.Combine(CliPaths.Log, "xabp-cli-logs.txt"), outputTemplate: loggerOutputTemplate)
            .WriteTo.Console(theme: AnsiConsoleTheme.Sixteen, outputTemplate: loggerOutputTemplate)
            .CreateLogger();

        using var application = await AbpApplicationFactory.CreateAsync<AbpCliModule>(
            options =>
            {
                options.UseAutofac();
                options.Services.AddLogging(c => c.AddSerilog());
            });
        await application.InitializeAsync();

        var newArgs = new List<string>();
#if DEBUG
        if (args.Any() && args.Length > 0)
        {
            newArgs = args.ToList();
            newArgs.AddIfNotContains("--skip-cli-version-check");
        }
        else
        {
            /*
            // newArgs.AddIfNotContains("generate-proxy");
            // newArgs.AddIfNotContains("-t");
            // newArgs.AddIfNotContains("ts");
            // newArgs.AddIfNotContains("-ast");
            // newArgs.AddIfNotContains("vben-axios");
            // newArgs.AddIfNotContains("-m");
            ////'accountAdmin','account','auditLogging','identity',
            ////'identityServer','languageManagement','textTemplateManagement',
            ////'chat','fileManagement','saas'
            // newArgs.AddIfNotContains("languageManagement");
            ////newArgs.AddIfNotContains("-o");
            ////newArgs.AddIfNotContains("api/identity");
            // newArgs.AddIfNotContains("-url");
            ////newArgs.AddIfNotContains("https://kf.4000871428.com/");
            // newArgs.AddIfNotContains("https://localhost:44302/");

            // newArgs.AddIfNotContains("generate-cert");

            // xabp create MyCompanyName.MyProjectName -pk MyPackageName -o "D:\Project" --dbms sqlserver --cs "Server=127.0.0.1;Database=MyProject;User Id=sa;Password=123456" --no-random-port
            //newArgs.AddIfNotContains("create");
            //newArgs.AddIfNotContains("Qing");
            //newArgs.AddIfNotContains("-o");
            //newArgs.AddIfNotContains("C:\\Project");
            //newArgs.AddIfNotContains("--dbms");
            //newArgs.AddIfNotContains("sqlserver");
            //newArgs.AddIfNotContains("--csf");

            // newArgs.AddIfNotContains("-v");
            // newArgs.AddIfNotContains("6.0.3");
            // newArgs.AddIfNotContains("--cs");
            // newArgs.AddIfNotContains("Server=127.0.0.1;Database=MyProject;User Id=sa;Password=123456");
            // newArgs.AddIfNotContains("--no-random-port");
            // newArgs.AddIfNotContains("MyPackageName");
            // newArgs.AddIfNotContains("MyPackageName");

            //// xabp install-module X.Payment VoloDocs Volo.Blogging
            //newArgs.AddIfNotContains("install-module");
            //newArgs.AddIfNotContains("X.Chat");
            //newArgs.AddIfNotContains("-s");
            //newArgs.AddIfNotContains("C:\\Code\\Test.Core\\BookStore\\Acme.BookStore");
            */
            // xabp list-modules
            // newArgs.AddIfNotContains("list-modules");
            // newArgs.AddIfNotContains("--include-pro-modules");
            newArgs.AddIfNotContains("show-version");

            newArgs.AddIfNotContains("--skip-cli-version-check");
        }
#else
                newArgs = args.ToList();
                newArgs.AddIfNotContains("--skip-cli-version-check");
#endif
        await application.ServiceProvider
            .GetRequiredService<CliService>()
            .RunAsync(newArgs.ToArray());

        await application.ShutdownAsync();

        Log.CloseAndFlush();
    }
}
