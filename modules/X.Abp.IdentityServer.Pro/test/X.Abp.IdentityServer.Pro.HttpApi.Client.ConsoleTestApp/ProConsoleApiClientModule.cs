using Volo.Abp.Autofac;
using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace X.Abp.IdentityServer.Pro;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(ProHttpApiClientModule),
    typeof(AbpHttpClientIdentityModelModule)
    )]
public class ProConsoleApiClientModule : AbpModule
{

}
