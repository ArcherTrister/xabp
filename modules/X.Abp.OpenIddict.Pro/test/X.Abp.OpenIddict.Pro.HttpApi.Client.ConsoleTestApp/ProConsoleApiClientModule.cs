using Volo.Abp.Autofac;
using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace X.Abp.OpenIddict;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpOpenIddictProHttpApiClientModule),
    typeof(AbpHttpClientIdentityModelModule)
    )]
public class ProConsoleApiClientModule : AbpModule
{

}
