using Volo.Abp.Modularity;

using X.Abp.Account;
using X.Abp.Identity;
using X.Abp.IdentityServer;

namespace MyCompanyName.MyProjectName.IdentityService;

[DependsOn(
    typeof(IdentityServiceApplicationContractsModule),
    typeof(AbpIdentityProHttpApiModule),
    typeof(AbpIdentityServerProHttpApiModule),
    typeof(AbpAccountAdminHttpApiModule)
    )]
public class IdentityServiceHttpApiModule : AbpModule
{

}
