// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.AutoMapper;
using Volo.Abp.Emailing;
using Volo.Abp.Modularity;
using Volo.Abp.UI.Navigation;
using Volo.Abp.VirtualFileSystem;
using Volo.CmsKit.Public;

namespace X.Abp.CmsKit.Public;

[DependsOn(
    typeof(CmsKitProDomainModule),
    typeof(AbpCmsKitProPublicApplicationContractsModule),
    typeof(CmsKitPublicApplicationModule),
    typeof(AbpEmailingModule),
    typeof(AbpUiNavigationModule))]
public class AbpCmsKitProPublicApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAutoMapperObjectMapper<AbpCmsKitProPublicApplicationModule>();
        Configure<AbpAutoMapperOptions>(options => options.AddMaps<AbpCmsKitProPublicApplicationModule>(true));
        Configure<AbpVirtualFileSystemOptions>(options => options.FileSets.AddEmbedded<AbpCmsKitProPublicApplicationModule>());
    }
}
