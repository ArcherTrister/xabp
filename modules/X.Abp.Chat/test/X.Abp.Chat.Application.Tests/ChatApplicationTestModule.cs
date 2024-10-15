using Volo.Abp.Modularity;

namespace X.Abp.Chat;

[DependsOn(
    typeof(AbpChatApplicationModule),
    typeof(ChatDomainTestModule)
    )]
public class ChatApplicationTestModule : AbpModule
{

}
