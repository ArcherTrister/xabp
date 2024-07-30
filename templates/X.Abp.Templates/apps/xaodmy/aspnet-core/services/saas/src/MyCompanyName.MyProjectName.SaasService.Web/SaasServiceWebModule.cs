using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

using X.Abp.Payment.Admin.Web;
using X.Abp.Saas.Web;

namespace MyCompanyName.MyProjectName.SaasService.Web;

[DependsOn(
    typeof(SaasServiceApplicationContractsModule),
    typeof(AbpSaasWebModule),
    typeof(AbpPaymentAdminWebModule))]
public class SaasServiceWebModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<SaasServiceWebModule>();
        });

        context.Services.AddAutoMapperObjectMapper<SaasServiceWebModule>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<SaasServiceWebModule>(validate: true);
        });
    }
}
