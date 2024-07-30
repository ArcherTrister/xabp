using Volo.Abp.FeatureManagement;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;

using X.Abp.AuditLogging;
using X.Abp.LanguageManagement;
using X.Abp.LeptonTheme.Management;
using X.Abp.TextTemplateManagement;

namespace MyCompanyName.MyProjectName.AdministrationService;

[DependsOn(
    typeof(AbpPermissionManagementApplicationContractsModule),
    typeof(AbpFeatureManagementApplicationContractsModule),
    typeof(AbpSettingManagementApplicationContractsModule),
    typeof(AbpAuditLoggingApplicationContractsModule),
    typeof(AbpLeptonThemeManagementApplicationContractsModule),
    typeof(AbpLanguageManagementApplicationContractsModule),
    typeof(AbpTextTemplateManagementApplicationContractsModule),
    typeof(AdministrationServiceDomainSharedModule)
)]
public class AdministrationServiceApplicationContractsModule : AbpModule
{
}
