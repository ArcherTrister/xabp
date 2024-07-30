using Volo.Abp.Autofac;
using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace X.Abp.VersionManagement;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(VersionManagementHttpApiClientModule),
    typeof(AbpHttpClientIdentityModelModule)
    )]
public class VersionManagementConsoleApiClientModule : AbpModule
{

}
