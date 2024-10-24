// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Volo.Abp;

namespace X.Abp.OpenIddict.HttpApi.Client.ConsoleTestApp;

public class ConsoleTestAppHostedService : IHostedService
{
  private readonly IConfiguration _configuration;

  public ConsoleTestAppHostedService(IConfiguration configuration)
  {
    _configuration = configuration;
  }

  public virtual async Task StartAsync(CancellationToken cancellationToken)
  {
    using (var application = await AbpApplicationFactory.CreateAsync<ProConsoleApiClientModule>(options =>
    {
      options.Services.ReplaceConfiguration(_configuration);
      options.UseAutofac();
    }))
    {
      await application.InitializeAsync();

      var demo = application.ServiceProvider.GetRequiredService<ClientDemoService>();
      await demo.RunAsync();

      await application.ShutdownAsync();
    }
  }

  public virtual Task StopAsync(CancellationToken cancellationToken)
  {
    return Task.CompletedTask;
  }
}
