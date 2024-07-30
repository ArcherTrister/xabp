using Volo.Abp.AspNetCore.Components.Server.Theming;
using Volo.Abp.Modularity;

namespace X.Abp.Quartz.Blazor.Server;

[DependsOn(
    typeof(AbpAspNetCoreComponentsServerThemingModule),
    typeof(QuartzBlazorModule)
    )]
public class QuartzBlazorServerModule : AbpModule
{

}
