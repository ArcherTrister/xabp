using Volo.Abp.Modularity;

namespace X.Abp.OpenIddict.Pro;

[DependsOn(
    typeof(ProApplicationModule),
    typeof(ProDomainTestModule)
    )]
public class ProApplicationTestModule : AbpModule
{

}
