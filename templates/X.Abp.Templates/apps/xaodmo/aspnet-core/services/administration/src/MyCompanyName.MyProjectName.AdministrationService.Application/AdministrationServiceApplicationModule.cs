using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.AutoMapper;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;

using X.Abp.AuditLogging;
using X.Abp.LanguageManagement;
using X.Abp.TextTemplateManagement;

namespace MyCompanyName.MyProjectName.AdministrationService;

[DependsOn(
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpSettingManagementApplicationModule),
    typeof(AbpAuditLoggingApplicationModule),
    typeof(AbpLanguageManagementApplicationModule),
    typeof(AbpTextTemplateManagementApplicationModule),
    typeof(AdministrationServiceApplicationContractsModule),
    typeof(AdministrationServiceDomainModule))]
public class AdministrationServiceApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAutoMapperObjectMapper<AdministrationServiceApplicationModule>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<AdministrationServiceApplicationModule>(validate: true);
        });
    }
}
