using Volo.Abp.AspNetCore.Components.WebAssembly.Theming;
using Volo.Abp.Modularity;

namespace X.Abp.Quartz.Blazor.WebAssembly;

[DependsOn(
    typeof(QuartzBlazorModule),
    typeof(AbpQuartzHttpApiClientModule),
    typeof(AbpAspNetCoreComponentsWebAssemblyThemingModule)
    )]
public class QuartzBlazorWebAssemblyModule : AbpModule
{

}
