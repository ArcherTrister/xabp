using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace X.Abp.Notification;

[DependsOn(
    typeof(AbpNotificationApplicationContractsModule),
    typeof(AbpHttpClientModule))]
public class AbpNotificationHttpApiClientModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddStaticHttpClientProxies(
            typeof(AbpNotificationApplicationContractsModule).Assembly,
            AbpNotificationRemoteServiceConsts.RemoteServiceName
        );

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<AbpNotificationHttpApiClientModule>();
        });
    }
}
