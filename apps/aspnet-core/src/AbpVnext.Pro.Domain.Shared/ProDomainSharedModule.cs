// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using AbpVnext.Pro.Localization;

using Volo.Abp.AuditLogging;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.BlobStoring.Database;
using Volo.Abp.FeatureManagement;
using Volo.Abp.GlobalFeatures;
using Volo.Abp.IdentityServer;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.VirtualFileSystem;

using X.Abp.Chat;
using X.Abp.CmsKit;
using X.Abp.FileManagement;
using X.Abp.Forms;
using X.Abp.Gdpr;
using X.Abp.Identity;
using X.Abp.LanguageManagement;
using X.Abp.Notification;
using X.Abp.OpenIddict;
using X.Abp.Payment;
using X.Abp.Saas;
using X.Abp.TextTemplateManagement;
using X.Abp.VersionManagement;

namespace AbpVnext.Pro;

[DependsOn(
    typeof(AbpAuditLoggingDomainSharedModule),
    typeof(AbpBackgroundJobsDomainSharedModule),
    typeof(AbpFeatureManagementDomainSharedModule),
    typeof(AbpIdentityProDomainSharedModule),
    typeof(AbpOpenIddictProDomainSharedModule),
    typeof(AbpIdentityServerDomainSharedModule),
    typeof(AbpPermissionManagementDomainSharedModule),
    typeof(AbpSettingManagementDomainSharedModule),
    typeof(AbpLanguageManagementDomainSharedModule),
    typeof(AbpSaasDomainSharedModule),
    typeof(AbpTextTemplateManagementDomainSharedModule),
    typeof(AbpGdprDomainSharedModule),
    typeof(AbpGlobalFeaturesModule),
    typeof(BlobStoringDatabaseDomainSharedModule))]

// [DependsOn(typeof(AbpBasicDataDomainSharedModule))]
[DependsOn(typeof(AbpNotificationDomainSharedModule))]

// [DependsOn(typeof(AbpIotDomainSharedModule))]
[DependsOn(typeof(AbpChatDomainSharedModule))]
[DependsOn(typeof(AbpFileManagementDomainSharedModule))]
[DependsOn(typeof(AbpFormsDomainSharedModule))]
[DependsOn(typeof(AbpCmsKitProDomainSharedModule))]
[DependsOn(typeof(AbpPaymentDomainSharedModule))]
[DependsOn(typeof(AbpVersionManagementDomainSharedModule))]
public class ProDomainSharedModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        ProGlobalFeatureConfigurator.Configure();
        ProModuleExtensionConfigurator.Configure();
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<ProDomainSharedModule>();
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Add<ProResource>("en")
                .AddVirtualJson("/Localization/Pro");

            options.DefaultResourceType = typeof(ProResource);
        });

        Configure<AbpExceptionLocalizationOptions>(options =>
        {
            options.MapCodeNamespace("AbpVnext.Pro", typeof(ProResource));
        });
    }
}
