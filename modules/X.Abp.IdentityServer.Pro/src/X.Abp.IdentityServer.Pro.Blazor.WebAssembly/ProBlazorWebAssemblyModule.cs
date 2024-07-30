using Volo.Abp.AspNetCore.Components.WebAssembly.Theming;
using Volo.Abp.Modularity;

namespace X.Abp.IdentityServer.Pro.Blazor.WebAssembly;

[DependsOn(
    typeof(ProBlazorModule),
    typeof(AbpIdentityServerHttpApiClientModule),
    typeof(AbpAspNetCoreComponentsWebAssemblyThemingModule)
    )]
public class ProBlazorWebAssemblyModule : AbpModule
{

}
