// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.AutoMapper;
using Volo.Abp.Emailing;
using Volo.Abp.Modularity;
using Volo.Abp.ObjectExtending.Modularity;
using Volo.Abp.SettingManagement;
using Volo.Abp.TextTemplating.Scriban;
using Volo.Abp.Threading;
using Volo.Abp.VirtualFileSystem;
using Volo.CmsKit;

using X.Abp.CmsKit.Newsletters;
using X.Abp.CmsKit.Polls;
using X.Abp.ObjectExtending;

namespace X.Abp.CmsKit;

[DependsOn(
    typeof(AbpCmsKitProDomainSharedModule),
    typeof(CmsKitDomainModule),
    typeof(AbpEmailingModule),
    typeof(AbpTextTemplatingScribanModule),
    typeof(AbpAutoMapperModule),
    typeof(AbpSettingManagementDomainModule))]
public class AbpCmsKitProDomainModule : AbpModule
{
    private static readonly OneTimeRunner OneTimeRunner = new OneTimeRunner();

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options => options.FileSets.AddEmbedded<AbpCmsKitProDomainModule>());
    }

    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        OneTimeRunner.Run(() =>
        {
            ModuleExtensionConfigurationHelper.ApplyEntityConfigurationToEntity(CmsKitProModuleExtensionConsts.ModuleName, CmsKitProModuleExtensionConsts.EntityNames.NewsletterRecord, typeof(NewsletterRecord));
            ModuleExtensionConfigurationHelper.ApplyEntityConfigurationToEntity(CmsKitProModuleExtensionConsts.ModuleName, CmsKitProModuleExtensionConsts.EntityNames.Poll, typeof(Poll));
        });
    }
}
