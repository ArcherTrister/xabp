using Volo.Abp.AspNetCore.Components.Server.Theming;
using Volo.Abp.Modularity;

namespace X.Abp.VersionManagement.Blazor.Server;

[DependsOn(
    typeof(AbpAspNetCoreComponentsServerThemingModule),
    typeof(VersionManagementBlazorModule)
    )]
public class VersionManagementBlazorServerModule : AbpModule
{

}
