using Volo.Abp.Autofac;
using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace X.Abp.Identity;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpIdentityProHttpApiClientModule),
    typeof(AbpHttpClientIdentityModelModule)
    )]
public class AbpIdentityProConsoleApiClientModule : AbpModule
{

}
