using Volo.Abp.Autofac;
using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace X.Abp.Forms;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpFormsHttpApiClientModule),
    typeof(AbpHttpClientIdentityModelModule)
    )]
public class FormsConsoleApiClientModule : AbpModule
{

}
