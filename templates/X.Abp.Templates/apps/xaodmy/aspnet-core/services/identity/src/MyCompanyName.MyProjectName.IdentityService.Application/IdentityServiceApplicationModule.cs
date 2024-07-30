using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

using X.Abp.Account;
using X.Abp.Identity;
using X.Abp.OpenIddict;

namespace MyCompanyName.MyProjectName.IdentityService;

[DependsOn(
    typeof(AbpIdentityProApplicationModule),
    typeof(AbpOpenIddictProApplicationModule),
    typeof(IdentityServiceDomainModule),
    typeof(AbpAccountAdminApplicationModule),
    typeof(IdentityServiceApplicationContractsModule))]
public class IdentityServiceApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAutoMapperObjectMapper<IdentityServiceApplicationModule>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<IdentityServiceApplicationModule>(validate: true);
        });
    }
}
