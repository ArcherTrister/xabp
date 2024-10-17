// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.AutoMapper;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;

using X.Abp.Account;
using X.Abp.AuditLogging;
using X.Abp.Chat;
using X.Abp.CmsKit;
using X.Abp.FileManagement;
using X.Abp.Forms;
using X.Abp.Gdpr;
using X.Abp.Identity;
using X.Abp.IdentityServer;
using X.Abp.LanguageManagement;
using X.Abp.Notification;
using X.Abp.OpenIddict;
using X.Abp.Payment;
using X.Abp.Payment.Admin;
using X.Abp.Quartz;
using X.Abp.Saas;
using X.Abp.TextTemplateManagement;
using X.Abp.VersionManagement;

namespace AbpVnext.Pro;

[DependsOn(
    typeof(ProDomainModule),
    typeof(ProApplicationContractsModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpSettingManagementApplicationModule))]
[DependsOn(typeof(AbpAccountPublicApplicationModule))]
[DependsOn(typeof(AbpAccountAdminApplicationModule))]
[DependsOn(typeof(AbpAuditLoggingApplicationModule))]

// [DependsOn(typeof(AbpBasicDataApplicationModule))]
[DependsOn(typeof(AbpChatApplicationModule))]
[DependsOn(typeof(AbpCmsKitProApplicationModule))]
[DependsOn(typeof(AbpFileManagementApplicationModule))]
[DependsOn(typeof(AbpFormsApplicationModule))]
[DependsOn(typeof(AbpGdprApplicationModule))]
[DependsOn(typeof(AbpIdentityProApplicationModule))]
[DependsOn(typeof(AbpIdentityServerProApplicationModule))]
[DependsOn(typeof(AbpOpenIddictProApplicationModule))]

// [DependsOn(typeof(AbpIotApplicationModule))]
[DependsOn(typeof(AbpLanguageManagementApplicationModule))]

// LeptonTheme
[DependsOn(typeof(AbpNotificationApplicationModule))]
[DependsOn(typeof(AbpPaymentAdminApplicationModule))]
[DependsOn(typeof(AbpPaymentApplicationModule))]
[DependsOn(typeof(AbpQuartzApplicationModule))]
[DependsOn(typeof(AbpSaasApplicationModule))]
[DependsOn(typeof(AbpTextTemplateManagementApplicationModule))]
[DependsOn(typeof(AbpVersionManagementApplicationModule))]
public class ProApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<ProApplicationModule>();
        });
    }
}
