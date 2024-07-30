using Volo.Abp.Modularity;

using X.Abp.Identity;
using X.Abp.OpenIddict;

namespace MyCompanyName.MyProjectName.IdentityService;

[DependsOn(
    typeof(AbpIdentityProDomainModule),
    typeof(AbpOpenIddictProDomainModule),
    typeof(IdentityServiceDomainSharedModule)
)]
public class IdentityServiceDomainModule : AbpModule
{
}
