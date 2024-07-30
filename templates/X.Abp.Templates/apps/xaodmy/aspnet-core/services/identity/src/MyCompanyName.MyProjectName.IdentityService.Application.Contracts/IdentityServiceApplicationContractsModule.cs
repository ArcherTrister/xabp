using Volo.Abp.Modularity;

using X.Abp.Account;
using X.Abp.Identity;
using X.Abp.OpenIddict;

namespace MyCompanyName.MyProjectName.IdentityService;

[DependsOn(
    typeof(AbpIdentityProApplicationContractsModule),
    typeof(AbpOpenIddictProApplicationContractsModule),
    typeof(AbpAccountAdminApplicationContractsModule),
    typeof(IdentityServiceDomainSharedModule))]
public class IdentityServiceApplicationContractsModule : AbpModule
{
}
