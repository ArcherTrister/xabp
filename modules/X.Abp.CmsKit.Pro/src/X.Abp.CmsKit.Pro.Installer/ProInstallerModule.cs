using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace X.Abp.CmsKit.Pro;

[DependsOn(
    typeof(AbpVirtualFileSystemModule)
    )]
public class ProInstallerModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<ProInstallerModule>();
        });
    }
}
