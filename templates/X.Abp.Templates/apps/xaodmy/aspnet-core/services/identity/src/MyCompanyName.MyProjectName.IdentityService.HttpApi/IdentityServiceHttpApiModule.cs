using Volo.Abp.Modularity;

using X.Abp.Account;
using X.Abp.Identity;
using X.Abp.OpenIddict;

namespace MyCompanyName.MyProjectName.IdentityService;

[DependsOn(
    typeof(IdentityServiceApplicationContractsModule),
    typeof(AbpIdentityProHttpApiModule),
    typeof(AbpOpenIddictProHttpApiModule),
    typeof(AbpAccountAdminHttpApiModule))]
public class IdentityServiceHttpApiModule : AbpModule
{
}
