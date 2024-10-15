using Volo.Abp.Autofac;
using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace X.Abp.LanguageManagement;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpLanguageManagementHttpApiClientModule),
    typeof(AbpHttpClientIdentityModelModule)
    )]
public class LanguageManagementConsoleApiClientModule : AbpModule
{

}
