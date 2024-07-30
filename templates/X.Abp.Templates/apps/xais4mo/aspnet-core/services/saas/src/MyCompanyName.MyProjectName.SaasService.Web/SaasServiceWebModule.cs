using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

using X.Abp.Payment.Admin.Web;
using X.Abp.Saas.Web;

namespace MyCompanyName.MyProjectName.SaasService.Web;

[DependsOn(
    typeof(SaasServiceApplicationContractsModule),
    typeof(AbpSaasWebModule),
    //typeof(SaasTenantWebModule),
    typeof(AbpPaymentAdminWebModule)
)]
public class SaasServiceWebModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<SaasServiceWebModule>();
        });
    }
}
