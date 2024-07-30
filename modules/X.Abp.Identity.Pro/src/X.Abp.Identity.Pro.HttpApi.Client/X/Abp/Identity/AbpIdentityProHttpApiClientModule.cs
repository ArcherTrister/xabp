// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace X.Abp.Identity;

[DependsOn(
    typeof(AbpIdentityProApplicationContractsModule),
    typeof(AbpHttpClientModule))]
public class AbpIdentityProHttpApiClientModule : AbpModule
{
    // public static readonly string Development = nameof(Development);
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddStaticHttpClientProxies(typeof(AbpIdentityProApplicationContractsModule).Assembly,
            AbpIdentityProRemoteServiceConsts.RemoteServiceName);

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<AbpIdentityProHttpApiClientModule>();
        });
    }
}
