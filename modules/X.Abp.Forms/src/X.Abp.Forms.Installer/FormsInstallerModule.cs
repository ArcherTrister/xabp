using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace X.Abp.Forms;

[DependsOn(
    typeof(AbpVirtualFileSystemModule)
    )]
public class FormsInstallerModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<FormsInstallerModule>();
        });
    }
}
