// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Threading.Tasks;

using Volo.Abp;

namespace MyCompanyName.MyProjectName.HttpApi.Client.ConsoleTestApp;

class Program
{
    static async Task Main(string[] args)
    {
        using (var application = await AbpApplicationFactory.CreateAsync<MyProjectNameConsoleApiClientModule>(options =>
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json", false);
            builder.AddJsonFile("appsettings.secrets.json", true);
            options.Services.ReplaceConfiguration(builder.Build());
            options.UseAutofac();
        }))
        {
            await application.InitializeAsync();

            var demo = application.ServiceProvider.GetRequiredService<ClientDemoService>();
            await demo.RunAsync();

            Console.WriteLine("Press ENTER to stop application...");
            Console.ReadLine();

            await application.ShutdownAsync();
        }
    }
}
