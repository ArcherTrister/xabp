using Volo.Abp.AuditLogging;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.HttpApi;
using Volo.Abp.SettingManagement;

using X.Abp.LanguageManagement;
using X.Abp.LeptonTheme.Management;
using X.Abp.TextTemplateManagement;

namespace MyCompanyName.MyProjectName.AdministrationService;

[DependsOn(
    typeof(AdministrationServiceApplicationContractsModule),
    typeof(AbpPermissionManagementHttpApiModule),
    typeof(AbpFeatureManagementHttpApiModule),
    typeof(AbpSettingManagementHttpApiModule),
    typeof(AbpAuditLoggingHttpApiModule),
    typeof(AbpLeptonThemeManagementHttpApiModule),
    typeof(AbpLanguageManagementHttpApiModule),
    typeof(AbpTextTemplateManagementHttpApiModule)
)]
public class AdministrationServiceHttpApiModule : AbpModule
{
}
