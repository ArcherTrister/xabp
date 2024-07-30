using Volo.Abp.AspNetCore.Components.Server.Theming;
using Volo.Abp.Modularity;

namespace X.Abp.Gdpr.Blazor.Server;

[DependsOn(
    typeof(AbpAspNetCoreComponentsServerThemingModule),
    typeof(GdprBlazorModule)
    )]
public class GdprBlazorServerModule : AbpModule
{

}
