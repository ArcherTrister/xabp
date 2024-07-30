using Microsoft.Extensions.DependencyInjection;

using MyCompanyName.MyProjectName.Shared.Hosting.AspNetCore;

using Volo.Abp.AspNetCore.Mvc.UI.MultiTenancy;
using Volo.Abp.Modularity;
using Volo.Abp.Swashbuckle;

namespace MyCompanyName.MyProjectName.Shared.Hosting.Gateways;

[DependsOn(
    typeof(MyProjectNameSharedHostingAspNetCoreModule),
    typeof(AbpAspNetCoreMvcUiMultiTenancyModule),
    typeof(AbpSwashbuckleModule)
)]
public class MyProjectNameSharedHostingGatewaysModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        var env = context.Services.GetHostingEnvironment();

        context.Services.AddReverseProxy()
            .LoadFromConfig(configuration.GetSection("ReverseProxy"));
    }
}
