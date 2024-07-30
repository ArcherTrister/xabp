using Volo.Abp.Modularity;
using Volo.Abp.Studio;
using Volo.Abp.VirtualFileSystem;

namespace X.Abp.Account;

[DependsOn(
    typeof(AbpStudioModuleInstallerModule),
    typeof(AbpVirtualFileSystemModule)
    )]
public class AccountProInstallerModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<AccountProInstallerModule>();
        });
    }
}
