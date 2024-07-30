using Volo.Abp.AspNetCore.Components.WebAssembly.Theming;
using Volo.Abp.Modularity;

namespace X.Abp.FileManagement.Blazor.WebAssembly;

[DependsOn(
    typeof(FileManagementBlazorModule),
    typeof(FileManagementHttpApiClientModule),
    typeof(AbpAspNetCoreComponentsWebAssemblyThemingModule)
    )]
public class FileManagementBlazorWebAssemblyModule : AbpModule
{

}
