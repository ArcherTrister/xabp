using Volo.Abp.Autofac;
using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace X.Abp.Saas;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(SaasHttpApiClientModule),
    typeof(AbpHttpClientIdentityModelModule)
    )]
public class SaasConsoleApiClientModule : AbpModule
{

}
