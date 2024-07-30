using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace X.Abp.Payment;

[DependsOn(
    typeof(AbpVirtualFileSystemModule)
    )]
public class PaymentInstallerModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<PaymentInstallerModule>();
        });
    }
}
