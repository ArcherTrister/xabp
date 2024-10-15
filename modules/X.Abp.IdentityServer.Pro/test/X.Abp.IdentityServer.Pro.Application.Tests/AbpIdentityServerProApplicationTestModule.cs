using Volo.Abp.Modularity;

namespace X.Abp.IdentityServer;

[DependsOn(
    typeof(AbpIdentityServerProApplicationModule),
    typeof(AbpIdentityServerProDomainTestModule)
    )]
public class AbpIdentityServerProApplicationTestModule : AbpModule
{

}
