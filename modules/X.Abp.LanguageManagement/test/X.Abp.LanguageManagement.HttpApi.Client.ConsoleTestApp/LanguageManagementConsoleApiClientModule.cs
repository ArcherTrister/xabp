using Volo.Abp.Autofac;
using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace X.Abp.LanguageManagement;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(LanguageManagementHttpApiClientModule),
    typeof(AbpHttpClientIdentityModelModule)
    )]
public class LanguageManagementConsoleApiClientModule : AbpModule
{

}
