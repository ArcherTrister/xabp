using Volo.Abp.AspNetCore.Components.WebAssembly.Theming;
using Volo.Abp.Modularity;

namespace X.Abp.VersionManagement.Blazor.WebAssembly;

[DependsOn(
    typeof(VersionManagementBlazorModule),
    typeof(VersionManagementHttpApiClientModule),
    typeof(AbpAspNetCoreComponentsWebAssemblyThemingModule)
    )]
public class VersionManagementBlazorWebAssemblyModule : AbpModule
{

}
