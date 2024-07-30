using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.FeatureManagement;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;

using X.Abp.AuditLogging;
using X.Abp.LanguageManagement;
using X.Abp.TextTemplateManagement;

namespace MyCompanyName.MyProjectName.AdministrationService;

[DependsOn(
    typeof(AdministrationServiceApplicationContractsModule),
    typeof(AbpPermissionManagementHttpApiClientModule),
    typeof(AbpFeatureManagementHttpApiClientModule),
    typeof(AbpSettingManagementHttpApiClientModule),
    typeof(AbpAuditLoggingHttpApiClientModule),
    typeof(AbpLanguageManagementHttpApiClientModule),
    typeof(AbpTextTemplateManagementHttpApiClientModule)
)]
public class AdministrationServiceHttpApiClientModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddHttpClientProxies(
            typeof(AdministrationServiceApplicationContractsModule).Assembly,
            AdministrationServiceRemoteServiceConsts.RemoteServiceName);
    }
}
