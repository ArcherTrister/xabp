using Volo.Abp.AspNetCore.Components.WebAssembly.Theming;
using Volo.Abp.Modularity;

namespace X.Abp.Chat.Blazor.WebAssembly;

[DependsOn(
    typeof(ChatBlazorModule),
    typeof(ChatHttpApiClientModule),
    typeof(AbpAspNetCoreComponentsWebAssemblyThemingModule)
    )]
public class ChatBlazorWebAssemblyModule : AbpModule
{

}
