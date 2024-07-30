using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace X.Abp.LanguageManagement;

[DependsOn(
    typeof(AbpVirtualFileSystemModule)
    )]
public class LanguageManagementInstallerModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<LanguageManagementInstallerModule>();
        });
    }
}
