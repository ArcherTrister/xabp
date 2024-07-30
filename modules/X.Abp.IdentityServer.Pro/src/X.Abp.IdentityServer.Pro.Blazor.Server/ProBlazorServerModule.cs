using Volo.Abp.AspNetCore.Components.Server.Theming;
using Volo.Abp.Modularity;

namespace X.Abp.IdentityServer.Pro.Blazor.Server;

[DependsOn(
    typeof(AbpAspNetCoreComponentsServerThemingModule),
    typeof(ProBlazorModule)
    )]
public class ProBlazorServerModule : AbpModule
{

}
