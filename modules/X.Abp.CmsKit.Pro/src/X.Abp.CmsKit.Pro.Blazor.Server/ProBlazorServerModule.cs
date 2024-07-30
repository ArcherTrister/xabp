using Volo.Abp.AspNetCore.Components.Server.Theming;
using Volo.Abp.Modularity;

namespace X.Abp.CmsKit.Pro.Blazor.Server;

[DependsOn(
    typeof(AbpAspNetCoreComponentsServerThemingModule),
    typeof(ProBlazorModule)
    )]
public class ProBlazorServerModule : AbpModule
{

}
