using Volo.Abp.Modularity;

namespace X.Abp.Forms;

[DependsOn(
    typeof(AbpFormsApplicationModule),
    typeof(FormsDomainTestModule)
    )]
public class FormsApplicationTestModule : AbpModule
{

}
