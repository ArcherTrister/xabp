using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace X.Abp.FileManagement;

[DependsOn(
    typeof(AbpVirtualFileSystemModule)
    )]
public class FileManagementInstallerModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<FileManagementInstallerModule>();
        });
    }
}
