// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Volo.Abp;

namespace AbpVnext.Pro.HttpApi.Client.ConsoleTestApp;

internal class Program
{
    private static async Task Main(string[] args)
    {
        using var application = await AbpApplicationFactory.CreateAsync<ProConsoleApiClientModule>(options =>
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json", false);
            builder.AddJsonFile("appsettings.secrets.json", true);
            options.Services.ReplaceConfiguration(builder.Build());
            options.UseAutofac();
        });
        await application.InitializeAsync();

        var demo = application.ServiceProvider.GetRequiredService<ClientDemoService>();
        await demo.RunAsync();

        Console.WriteLine("Press ENTER to stop application...");
        Console.ReadLine();

        await application.ShutdownAsync();
    }
}
