using Volo.Abp.Autofac;
using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace X.Abp.IdentityServer;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpIdentityServerProHttpApiClientModule),
    typeof(AbpHttpClientIdentityModelModule)
    )]
public class AbpIdentityServerProConsoleApiClientModule : AbpModule
{

}
