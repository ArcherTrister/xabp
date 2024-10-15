using Volo.Abp.Autofac;
using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace X.Abp.Quartz;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpQuartzHttpApiClientModule),
    typeof(AbpHttpClientIdentityModelModule)
    )]
public class QuartzConsoleApiClientModule : AbpModule
{

}
