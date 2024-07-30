using Volo.Abp.Modularity;

namespace X.Abp.Identity.Pro;

[DependsOn(
    typeof(ProApplicationModule),
    typeof(ProDomainTestModule)
    )]
public class ProApplicationTestModule : AbpModule
{

}
