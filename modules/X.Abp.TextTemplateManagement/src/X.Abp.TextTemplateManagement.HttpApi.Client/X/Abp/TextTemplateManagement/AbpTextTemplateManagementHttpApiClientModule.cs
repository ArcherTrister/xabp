// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace X.Abp.TextTemplateManagement;

[DependsOn(
    typeof(AbpTextTemplateManagementApplicationContractsModule),
    typeof(AbpHttpClientModule))]
public class AbpTextTemplateManagementHttpApiClientModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddHttpClientProxies(
            typeof(AbpTextTemplateManagementApplicationContractsModule).Assembly,
            AbpTextTemplateManagementRemoteServiceConsts.RemoteServiceName);

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<AbpTextTemplateManagementHttpApiClientModule>();
        });
    }
}
