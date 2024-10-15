using Volo.Abp.Modularity;

namespace X.Abp.Saas;

[DependsOn(
    typeof(AbpSaasApplicationModule),
    typeof(SaasDomainTestModule)
    )]
public class SaasApplicationTestModule : AbpModule
{

}
