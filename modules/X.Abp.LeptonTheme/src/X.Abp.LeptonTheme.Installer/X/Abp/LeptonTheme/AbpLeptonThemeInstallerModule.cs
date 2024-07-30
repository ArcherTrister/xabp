using Volo.Abp.Modularity;
using Volo.Abp.Studio;
using Volo.Abp.VirtualFileSystem;

namespace X.Abp.LeptonTheme
{
    [DependsOn(
        typeof(AbpStudioModuleInstallerModule),
        typeof(AbpVirtualFileSystemModule)
        )]
    public class AbpLeptonThemeInstallerModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<AbpLeptonThemeInstallerModule>();
            });
        }
    }
}
