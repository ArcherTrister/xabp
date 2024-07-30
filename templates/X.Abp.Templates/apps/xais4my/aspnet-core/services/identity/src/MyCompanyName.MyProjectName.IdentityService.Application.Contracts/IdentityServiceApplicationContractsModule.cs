using Volo.Abp.Modularity;

using X.Abp.Account;
using X.Abp.Identity;
using X.Abp.IdentityServer;

namespace MyCompanyName.MyProjectName.IdentityService;

[DependsOn(
    typeof(AbpIdentityProApplicationContractsModule),
    typeof(AbpIdentityServerProApplicationContractsModule),
    typeof(AbpAccountAdminApplicationContractsModule),
    typeof(IdentityServiceDomainSharedModule)
)]
public class IdentityServiceApplicationContractsModule : AbpModule
{
}
