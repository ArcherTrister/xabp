using Volo.Abp.Autofac;
using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace X.Abp.CmsKit.Pro;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(CmsKitProHttpApiClientModule),
    typeof(AbpHttpClientIdentityModelModule)
    )]
public class CmsKitProConsoleApiClientModule : AbpModule
{

}
