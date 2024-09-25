using Volo.Abp.FeatureManagement;
using Volo.Abp.Modularity;
using Volo.Abp.ObjectExtending;
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
    typeof(MyProjectNameDomainSharedModule),
    typeof(AbpFeatureManagementApplicationContractsModule),
    typeof(AbpPermissionManagementApplicationContractsModule),
    typeof(AbpSettingManagementApplicationContractsModule),
    typeof(AbpSaasApplicationContractsModule),
    typeof(AbpObjectExtendingModule),
    typeof(AbpIdentityProApplicationContractsModule),
#if IdentityServer4
    typeof(AbpIdentityServerProApplicationContractsModule),
#else
    typeof(AbpOpenIddictProApplicationContractsModule),
#endif
    typeof(AbpAuditLoggingApplicationContractsModule),
    typeof(AbpAccountPublicApplicationContractsModule),
    typeof(AbpAccountAdminApplicationContractsModule),
    typeof(AbpFileManagementApplicationContractsModule),
    typeof(AbpLanguageManagementApplicationContractsModule),
    typeof(AbpGdprApplicationContractsModule),
    typeof(AbpTextTemplateManagementApplicationContractsModule))]
public class MyProjectNameApplicationContractsModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        MyProjectNameDtoExtensions.Configure();
    }
}
