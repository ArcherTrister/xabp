using Volo.Abp.Modularity;

namespace X.Abp.IdentityServer.Pro;

[DependsOn(
    typeof(ProApplicationModule),
    typeof(ProDomainTestModule)
    )]
public class ProApplicationTestModule : AbpModule
{

}
