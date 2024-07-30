using Volo.Abp.Modularity;

using X.Abp.Identity;
using X.Abp.IdentityServer;

namespace MyCompanyName.MyProjectName.IdentityService;

[DependsOn(
    typeof(AbpIdentityProDomainModule),
    typeof(AbpIdentityServerProDomainModule),
    typeof(IdentityServiceDomainSharedModule))]
public class IdentityServiceDomainModule : AbpModule
{
}
