using Volo.Abp.FeatureManagement;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.HttpApi;
using Volo.Abp.SettingManagement;

using X.Abp.AuditLogging;
using X.Abp.LanguageManagement;
using X.Abp.TextTemplateManagement;

namespace MyCompanyName.MyProjectName.AdministrationService;

[DependsOn(
    typeof(AdministrationServiceApplicationContractsModule),
    typeof(AbpPermissionManagementHttpApiModule),
    typeof(AbpFeatureManagementHttpApiModule),
    typeof(AbpSettingManagementHttpApiModule),
    typeof(AbpAuditLoggingHttpApiModule),
    typeof(AbpLanguageManagementHttpApiModule),
    typeof(AbpTextTemplateManagementHttpApiModule))]
public class AdministrationServiceHttpApiModule : AbpModule
{
}
