using Volo.Abp.Autofac;
using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace X.Abp.Chat;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(ChatHttpApiClientModule),
    typeof(AbpHttpClientIdentityModelModule)
    )]
public class ChatConsoleApiClientModule : AbpModule
{

}
