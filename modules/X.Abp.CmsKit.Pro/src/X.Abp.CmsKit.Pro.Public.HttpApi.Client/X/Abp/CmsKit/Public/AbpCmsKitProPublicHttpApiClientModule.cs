// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;
using Volo.CmsKit.Public;

namespace X.Abp.CmsKit.Public;

[DependsOn(
    typeof(AbpCmsKitProPublicApplicationContractsModule),
    typeof(CmsKitPublicHttpApiClientModule))]
public class AbpCmsKitProPublicHttpApiClientModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddStaticHttpClientProxies(typeof(AbpCmsKitProPublicApplicationContractsModule).Assembly, AbpCmsKitProPublicRemoteServiceConsts.RemoteServiceName);
        Configure<AbpVirtualFileSystemOptions>(options => options.FileSets.AddEmbedded<AbpCmsKitProPublicHttpApiClientModule>());
    }
}
