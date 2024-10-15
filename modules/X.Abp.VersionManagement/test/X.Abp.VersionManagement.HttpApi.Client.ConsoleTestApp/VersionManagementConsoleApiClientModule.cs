using Volo.Abp.Autofac;
using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace X.Abp.VersionManagement;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpVersionManagementHttpApiClientModule),
    typeof(AbpHttpClientIdentityModelModule)
    )]
public class VersionManagementConsoleApiClientModule : AbpModule
{

}
