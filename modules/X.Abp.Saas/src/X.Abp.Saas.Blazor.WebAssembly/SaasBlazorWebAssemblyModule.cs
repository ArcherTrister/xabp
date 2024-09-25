using Volo.Abp.AspNetCore.Components.WebAssembly.Theming;
using Volo.Abp.Modularity;

namespace X.Abp.Saas.Blazor.WebAssembly;

[DependsOn(
    typeof(SaasBlazorModule),
    typeof(AbpSaasHttpApiClientModule),
    typeof(AbpAspNetCoreComponentsWebAssemblyThemingModule)
    )]
public class SaasBlazorWebAssemblyModule : AbpModule
{

}
