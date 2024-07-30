// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace X.Abp.Payment.Admin
{
    [DependsOn(
        typeof(AbpPaymentAdminApplicationContractsModule),
        typeof(AbpPaymentHttpApiClientModule))]
    public class AbpPaymentAdminHttpApiClientModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            // 3
            context.Services.AddStaticHttpClientProxies(typeof(AbpPaymentAdminApplicationContractsModule).Assembly, AbpPaymentAdminRemoteServiceConsts.RemoteServiceName);

            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<AbpPaymentAdminHttpApiClientModule>();
            });
        }
    }
}
