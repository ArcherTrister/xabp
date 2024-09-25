using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.AutoMapper;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;

using X.Abp.Account;
using X.Abp.AuditLogging;
using X.Abp.FileManagement;
using X.Abp.Gdpr;
using X.Abp.Identity;
#if IdentityServer4
using X.Abp.IdentityServer;
#else
using X.Abp.OpenIddict;
#endif
using X.Abp.LanguageManagement;
using X.Abp.Saas;
using X.Abp.TextTemplateManagement;

namespace MyCompanyName.MyProjectName;

[DependsOn(
    typeof(MyProjectNameDomainModule),
    typeof(MyProjectNameApplicationContractsModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpSettingManagementApplicationModule),
    typeof(AbpIdentityProApplicationModule),
#if IdentityServer4
    typeof(AbpIdentityServerProApplicationModule),
#else
    typeof(AbpOpenIddictProApplicationModule),
#endif
    typeof(AbpSaasApplicationModule),
    typeof(AbpAuditLoggingApplicationModule),
    typeof(AbpAccountPublicApplicationModule),
    typeof(AbpAccountAdminApplicationModule),
    typeof(AbpFileManagementApplicationModule),
    typeof(AbpLanguageManagementApplicationModule),
    typeof(AbpGdprApplicationModule),
    typeof(AbpTextTemplateManagementApplicationModule)
    )]
public class MyProjectNameApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAutoMapperObjectMapper<MyProjectNameApplicationModule>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<MyProjectNameApplicationModule>();
        });
    }
}
