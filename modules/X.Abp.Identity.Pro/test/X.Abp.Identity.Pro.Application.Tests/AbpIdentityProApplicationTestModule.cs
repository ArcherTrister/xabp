using Volo.Abp.Modularity;

namespace X.Abp.Identity;

[DependsOn(
    typeof(AbpIdentityProApplicationModule),
    typeof(AbpIdentityProDomainTestModule)
    )]
public class AbpIdentityProApplicationTestModule : AbpModule
{

}
