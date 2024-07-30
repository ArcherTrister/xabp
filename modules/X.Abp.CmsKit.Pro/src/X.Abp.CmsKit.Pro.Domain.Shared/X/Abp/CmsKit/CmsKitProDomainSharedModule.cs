// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;
using Volo.CmsKit;
using Volo.CmsKit.Localization;

namespace X.Abp.CmsKit;

[DependsOn(typeof(CmsKitDomainSharedModule))]
public class CmsKitProDomainSharedModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options => options.FileSets.AddEmbedded<CmsKitProDomainSharedModule>());
        Configure<AbpLocalizationOptions>(options => options.Resources.Get<CmsKitResource>().AddVirtualJson("/X/Abp/CmsKit/Localization/Resources"));
        Configure<AbpExceptionLocalizationOptions>(options => options.MapCodeNamespace("CmsKitPro", typeof(CmsKitResource)));
    }
}
