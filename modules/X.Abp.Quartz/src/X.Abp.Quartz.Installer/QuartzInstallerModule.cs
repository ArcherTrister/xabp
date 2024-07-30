using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace X.Abp.Quartz;

[DependsOn(
    typeof(AbpVirtualFileSystemModule)
    )]
public class QuartzInstallerModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<QuartzInstallerModule>();
        });
    }
}
