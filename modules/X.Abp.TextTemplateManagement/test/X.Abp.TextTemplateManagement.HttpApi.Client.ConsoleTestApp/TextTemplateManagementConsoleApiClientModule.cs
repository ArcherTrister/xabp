using Volo.Abp.Autofac;
using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace X.Abp.TextTemplateManagement;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpTextTemplateManagementHttpApiClientModule),
    typeof(AbpHttpClientIdentityModelModule)
    )]
public class TextTemplateManagementConsoleApiClientModule : AbpModule
{

}
