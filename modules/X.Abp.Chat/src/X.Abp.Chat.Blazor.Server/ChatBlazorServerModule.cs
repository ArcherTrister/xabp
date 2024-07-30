using Volo.Abp.AspNetCore.Components.Server.Theming;
using Volo.Abp.Modularity;

namespace X.Abp.Chat.Blazor.Server;

[DependsOn(
    typeof(AbpAspNetCoreComponentsServerThemingModule),
    typeof(ChatBlazorModule)
    )]
public class ChatBlazorServerModule : AbpModule
{

}
