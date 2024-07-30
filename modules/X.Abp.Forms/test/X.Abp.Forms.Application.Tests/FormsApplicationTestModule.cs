using Volo.Abp.Modularity;

namespace X.Abp.Forms;

[DependsOn(
    typeof(FormsApplicationModule),
    typeof(FormsDomainTestModule)
    )]
public class FormsApplicationTestModule : AbpModule
{

}
