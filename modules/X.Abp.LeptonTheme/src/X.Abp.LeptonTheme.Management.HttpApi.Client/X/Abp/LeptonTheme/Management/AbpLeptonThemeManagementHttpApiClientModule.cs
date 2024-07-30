using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace X.Abp.LeptonTheme.Management
{
    [DependsOn(
        typeof(AbpLeptonThemeManagementApplicationContractsModule),
        typeof(AbpHttpClientModule)
        )]
    public class AbpLeptonThemeManagementHttpApiClientModule : AbpModule
    {
        public const string RemoteServiceName = "LeptonTheme";

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddStaticHttpClientProxies(
                typeof(AbpLeptonThemeManagementApplicationContractsModule).Assembly,
                RemoteServiceName
            );

            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<AbpLeptonThemeManagementHttpApiClientModule>();
            });
        }
    }
}
