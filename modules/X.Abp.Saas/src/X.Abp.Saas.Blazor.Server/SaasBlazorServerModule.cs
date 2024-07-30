using Volo.Abp.AspNetCore.Components.Server.Theming;
using Volo.Abp.Modularity;

namespace X.Abp.Saas.Blazor.Server;

[DependsOn(
    typeof(AbpAspNetCoreComponentsServerThemingModule),
    typeof(SaasBlazorModule)
    )]
public class SaasBlazorServerModule : AbpModule
{

}
