using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.Modularity;

using X.Abp.Account;
using X.Abp.Identity;
using X.Abp.OpenIddict;

namespace MyCompanyName.MyProjectName.IdentityService;

[DependsOn(
    typeof(IdentityServiceApplicationContractsModule),
    typeof(AbpOpenIddictProHttpApiClientModule),
    typeof(AbpIdentityProHttpApiClientModule),
    typeof(AbpAccountAdminHttpApiClientModule))]
public class IdentityServiceHttpApiClientModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddStaticHttpClientProxies(
            typeof(IdentityServiceApplicationContractsModule).Assembly,
            IdentityServiceRemoteServiceConsts.RemoteServiceName
        );
    }
}
