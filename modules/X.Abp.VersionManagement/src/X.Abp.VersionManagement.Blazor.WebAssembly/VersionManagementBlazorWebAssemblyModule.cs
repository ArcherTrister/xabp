using Volo.Abp.AspNetCore.Components.WebAssembly.Theming;
using Volo.Abp.Modularity;

namespace X.Abp.VersionManagement.Blazor.WebAssembly;

[DependsOn(
    typeof(VersionManagementBlazorModule),
    typeof(AbpVersionManagementHttpApiClientModule),
    typeof(AbpAspNetCoreComponentsWebAssemblyThemingModule)
    )]
public class VersionManagementBlazorWebAssemblyModule : AbpModule
{

}
