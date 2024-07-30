using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace X.Abp.Gdpr;

[DependsOn(
    typeof(AbpVirtualFileSystemModule)
    )]
public class GdprInstallerModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<GdprInstallerModule>();
        });
    }
}
