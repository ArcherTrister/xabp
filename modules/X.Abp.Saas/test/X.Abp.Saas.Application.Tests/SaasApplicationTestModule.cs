using Volo.Abp.Modularity;

namespace X.Abp.Saas;

[DependsOn(
    typeof(SaasApplicationModule),
    typeof(SaasDomainTestModule)
    )]
public class SaasApplicationTestModule : AbpModule
{

}
