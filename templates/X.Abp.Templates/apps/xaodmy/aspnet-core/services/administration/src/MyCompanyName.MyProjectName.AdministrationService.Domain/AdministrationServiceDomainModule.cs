using Volo.Abp.AuditLogging;
using Volo.Abp.FeatureManagement;
using Volo.Abp.LanguageManagement;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.PermissionManagement.Identity;
using Volo.Abp.PermissionManagement.OpenIddict;
using Volo.Abp.SettingManagement;

using X.Abp.TextTemplateManagement;

namespace MyCompanyName.MyProjectName.AdministrationService;

[DependsOn(
    typeof(AdministrationServiceDomainSharedModule),
    typeof(AbpPermissionManagementDomainModule),
    typeof(AbpFeatureManagementDomainModule),
    typeof(AbpSettingManagementDomainModule),
    typeof(AbpAuditLoggingDomainModule),
    typeof(LanguageManagementDomainModule),
    typeof(AbpTextTemplateManagementDomainModule),
    typeof(AbpPermissionManagementDomainOpenIddictModule),
    typeof(AbpPermissionManagementDomainIdentityModule)
)]
public class AdministrationServiceDomainModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            /* These languages are used on data seed. If you add new, you need to run the seed data */
            options.Languages.Add(new LanguageInfo("ar", "ar", "العربية", "ae"));
            options.Languages.Add(new LanguageInfo("cs", "cs", "Čeština"));
            options.Languages.Add(new LanguageInfo("en", "en", "English"));
            options.Languages.Add(new LanguageInfo("fi", "fi", "Finnish", "fi"));
            options.Languages.Add(new LanguageInfo("fr", "fr", "Français", "fr"));
            options.Languages.Add(new LanguageInfo("hi", "hi", "Hindi", "in"));
            options.Languages.Add(new LanguageInfo("it", "it", "Italiano", "it"));
            options.Languages.Add(new LanguageInfo("sl", "sl", "Slovenščina"));
            options.Languages.Add(new LanguageInfo("sk", "sk", "Slovak", "sk"));
            options.Languages.Add(new LanguageInfo("ru", "ru", "Русский", "ru"));
            options.Languages.Add(new LanguageInfo("tr", "tr", "Türkçe"));
            options.Languages.Add(new LanguageInfo("zh-Hans", "zh-Hans", "简体中文"));
            options.Languages.Add(new LanguageInfo("zh-Hant", "zh-Hant", "繁體中文"));
            options.Languages.Add(new LanguageInfo("de-DE", "de-DE", "Deutsch", "de"));
            options.Languages.Add(new LanguageInfo("es", "es", "Español", "es"));
        });
    }
}
