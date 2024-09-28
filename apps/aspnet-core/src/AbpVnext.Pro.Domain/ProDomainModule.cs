// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using AbpVnext.Pro.Localization;
using AbpVnext.Pro.MultiTenancy;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Volo.Abp.AuditLogging;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.BlobStoring.Database;
using Volo.Abp.Emailing;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement.Identity;
using Volo.Abp.PermissionManagement.IdentityServer;
using Volo.Abp.PermissionManagement.OpenIddict;
using Volo.Abp.SettingManagement;
using Volo.Abp.Sms;

using X.Abp.Chat;
using X.Abp.CmsKit;
using X.Abp.CmsKit.Newsletters;
using X.Abp.FileManagement;
using X.Abp.Forms;
using X.Abp.Gdpr;
using X.Abp.Identity;
using X.Abp.IdentityServer;
using X.Abp.LanguageManagement;
using X.Abp.Notification;
using X.Abp.OpenIddict;
using X.Abp.Payment;
using X.Abp.Saas;
using X.Abp.TextTemplateManagement;
using X.Abp.VersionManagement;

namespace AbpVnext.Pro;

[DependsOn(
    typeof(ProDomainSharedModule),
    typeof(AbpAuditLoggingDomainModule),
    typeof(AbpBackgroundJobsDomainModule),
    typeof(AbpFeatureManagementDomainModule),
    typeof(AbpIdentityProDomainModule),
    typeof(AbpPermissionManagementDomainIdentityModule),
    typeof(AbpOpenIddictProDomainModule),
    typeof(AbpPermissionManagementDomainOpenIddictModule),
    typeof(AbpIdentityServerProDomainModule),
    typeof(AbpPermissionManagementDomainIdentityServerModule),
    typeof(AbpSettingManagementDomainModule),
    typeof(AbpSaasDomainModule),
    typeof(AbpTextTemplateManagementDomainModule),
    typeof(AbpLanguageManagementDomainModule),
    typeof(AbpEmailingModule),
    typeof(AbpSmsModule),
    typeof(AbpGdprDomainModule),
    typeof(BlobStoringDatabaseDomainModule))]

// [DependsOn(typeof(AbpAccountSharedIdentityServerModule))]
// [DependsOn(typeof(AbpBasicDataDomainModule))]
[DependsOn(typeof(AbpNotificationDomainModule))]

// [DependsOn(typeof(AbpIotDomainModule))]
[DependsOn(typeof(AbpChatDomainModule))]
[DependsOn(typeof(AbpFileManagementDomainModule))]
[DependsOn(typeof(AbpFormsDomainModule))]
[DependsOn(typeof(CmsKitProDomainModule))]
[DependsOn(typeof(AbpPaymentDomainModule))]
[DependsOn(typeof(AbpVersionManagementDomainModule))]
public class ProDomainModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpMultiTenancyOptions>(options =>
        {
            options.IsEnabled = MultiTenancyConsts.IsEnabled;
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Languages.Add(new LanguageInfo("ar", "ar", "العربية"));
            options.Languages.Add(new LanguageInfo("cs", "cs", "Čeština"));
            options.Languages.Add(new LanguageInfo("en", "en", "English"));
            options.Languages.Add(new LanguageInfo("fi", "fi", "Finnish"));
            options.Languages.Add(new LanguageInfo("fr", "fr", "Français"));
            options.Languages.Add(new LanguageInfo("hi", "hi", "Hindi"));
            options.Languages.Add(new LanguageInfo("it", "it", "Italiano"));
            options.Languages.Add(new LanguageInfo("sl", "sl", "Slovenščina"));
            options.Languages.Add(new LanguageInfo("sk", "sk", "Slovak"));
            options.Languages.Add(new LanguageInfo("ru", "ru", "Русский"));
            options.Languages.Add(new LanguageInfo("tr", "tr", "Türkçe"));
            options.Languages.Add(new LanguageInfo("zh-Hans", "zh-Hans", "简体中文"));
            options.Languages.Add(new LanguageInfo("zh-Hant", "zh-Hant", "繁體中文"));
            options.Languages.Add(new LanguageInfo("de-DE", "de-DE", "Deutsch"));
            options.Languages.Add(new LanguageInfo("es", "es", "Español"));
        });

        // Configure<AbpLocalizationCultureMapOptions>(options =>
        // {
        //    var zhHansCultureMapInfo = new CultureMapInfo
        //    {
        //        TargetCulture = "zh-Hans",
        //        SourceCultures = new string[] { "zh", "zh_CN", "zh_cn", "zh-CN", "zh-cn" }
        //    };
        // // TODO: zh-Hant
        //    options.CulturesMaps.Add(zhHansCultureMapInfo);
        //    options.UiCulturesMaps.Add(zhHansCultureMapInfo);
        // });
        Configure<NewsletterOptions>(options =>
        {
            options.AddPreference(
                "Newsletter_Default",
                new NewsletterPreferenceDefinition(
                    LocalizableString.Create<ProResource>("NewsletterPreference_Default"),
                    privacyPolicyConfirmation: LocalizableString.Create<ProResource>("NewsletterPrivacyAcceptMessage")));
        });

#if DEBUG
        context.Services.Replace(ServiceDescriptor.Singleton<IEmailSender, NullEmailSender>());
        context.Services.Replace(ServiceDescriptor.Singleton<ISmsSender, NullSmsSender>());
#endif
    }
}
