using Volo.Abp.Modularity;
using Volo.Abp.Studio;
using Volo.Abp.VirtualFileSystem;

namespace X.Chat;

[DependsOn(
    typeof(AbpStudioModuleInstallerModule),
    typeof(AbpVirtualFileSystemModule)
    )]
public class AbpIdentityProInstallerModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<AbpIdentityProInstallerModule>();
        });
    }
}
