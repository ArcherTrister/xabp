using Volo.Abp.Autofac;
using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace X.Abp.Gdpr;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(GdprHttpApiClientModule),
    typeof(AbpHttpClientIdentityModelModule)
    )]
public class GdprConsoleApiClientModule : AbpModule
{

}
