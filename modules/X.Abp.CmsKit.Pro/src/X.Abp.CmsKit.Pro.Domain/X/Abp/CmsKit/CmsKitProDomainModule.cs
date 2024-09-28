// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.AutoMapper;
using Volo.Abp.Emailing;
using Volo.Abp.Modularity;
using Volo.Abp.SettingManagement;
using Volo.Abp.TextTemplating.Scriban;
using Volo.Abp.VirtualFileSystem;
using Volo.CmsKit;

namespace X.Abp.CmsKit;

[DependsOn(
    typeof(CmsKitProDomainSharedModule),
    typeof(CmsKitDomainModule),
    typeof(AbpEmailingModule),
    typeof(AbpTextTemplatingScribanModule),
    typeof(AbpAutoMapperModule),
    typeof(AbpSettingManagementDomainModule))]
public class CmsKitProDomainModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options => options.FileSets.AddEmbedded<CmsKitProDomainModule>());
    }
}
