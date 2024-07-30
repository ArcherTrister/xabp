using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace X.Abp.Saas;

[DependsOn(
    typeof(AbpVirtualFileSystemModule)
    )]
public class SaasInstallerModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<SaasInstallerModule>();
        });
    }
}
