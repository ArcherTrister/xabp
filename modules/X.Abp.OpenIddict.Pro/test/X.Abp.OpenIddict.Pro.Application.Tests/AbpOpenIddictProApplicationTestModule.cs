using Volo.Abp.Modularity;

namespace X.Abp.OpenIddict;

[DependsOn(
    typeof(AbpOpenIddictProApplicationModule),
    typeof(AbpOpenIddictProDomainTestModule)
    )]
public class AbpOpenIddictProApplicationTestModule : AbpModule
{

}
