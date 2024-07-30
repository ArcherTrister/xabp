// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

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
    typeof(ProDomainSharedModule),
    typeof(AbpFeatureManagementApplicationContractsModule),
    typeof(AbpIdentityProApplicationContractsModule),
    typeof(AbpPermissionManagementApplicationContractsModule),
    typeof(AbpSettingManagementApplicationContractsModule),
    typeof(AbpSaasApplicationContractsModule),
    typeof(AbpAuditLoggingApplicationContractsModule),
    typeof(AbpOpenIddictProApplicationContractsModule),
    typeof(AbpIdentityServerProApplicationContractsModule),
    typeof(AbpAccountPublicApplicationContractsModule),
    typeof(AbpAccountAdminApplicationContractsModule),
    typeof(AbpLanguageManagementApplicationContractsModule),
    typeof(AbpGdprApplicationContractsModule),
    typeof(AbpTextTemplateManagementApplicationContractsModule))]
// [DependsOn(typeof(AbpBasicDataApplicationContractsModule))]
[DependsOn(typeof(AbpNotificationApplicationContractsModule))]
//[DependsOn(typeof(AbpIotApplicationContractsModule))]
[DependsOn(typeof(AbpOpenIddictProApplicationContractsModule))]
[DependsOn(typeof(AbpQuartzApplicationContractsModule))]
[DependsOn(typeof(AbpChatApplicationContractsModule))]
[DependsOn(typeof(AbpFileManagementApplicationContractsModule))]
[DependsOn(typeof(AbpFormsApplicationContractsModule))]

// [DependsOn(typeof(CmsKitProAdminApplicationContractsModule))]
[DependsOn(typeof(CmsKitProApplicationContractsModule))]

// [DependsOn(typeof(CmsKitProPublicApplicationContractsModule))]
[DependsOn(typeof(AbpPaymentAdminApplicationContractsModule))]
[DependsOn(typeof(AbpPaymentApplicationContractsModule))]
[DependsOn(typeof(AbpVersionManagementApplicationContractsModule))]
public class ProApplicationContractsModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        ProDtoExtensions.Configure();
    }
}
