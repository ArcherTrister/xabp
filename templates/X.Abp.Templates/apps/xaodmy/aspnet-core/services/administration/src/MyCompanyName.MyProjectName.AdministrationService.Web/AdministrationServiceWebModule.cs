using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

using X.Abp.Account.Admin.Web;
using X.Abp.AuditLogging.Web;
using X.Abp.LanguageManagement.Web;
using X.Abp.TextTemplateManagement.Web;

namespace MyCompanyName.MyProjectName.AdministrationService.Web;

[DependsOn(
    typeof(AbpAuditLoggingWebModule),
    typeof(AbpLanguageManagementWebModule),
    typeof(AbpTextTemplateManagementWebModule),
    typeof(AbpAccountAdminWebModule),
    typeof(AdministrationServiceApplicationContractsModule))]
public class AdministrationServiceWebModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<AdministrationServiceWebModule>();
        });

        context.Services.AddAutoMapperObjectMapper<AdministrationServiceWebModule>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<AdministrationServiceWebModule>(validate: true);
        });
    }
}
