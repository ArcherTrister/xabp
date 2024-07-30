using Volo.Abp.AspNetCore.Components.WebAssembly.Theming;
using Volo.Abp.Modularity;

namespace X.Abp.CmsKit.Pro.Blazor.WebAssembly;

[DependsOn(
    typeof(ProBlazorModule),
    typeof(ProHttpApiClientModule),
    typeof(AbpAspNetCoreComponentsWebAssemblyThemingModule)
    )]
public class ProBlazorWebAssemblyModule : AbpModule
{

}
