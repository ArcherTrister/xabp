// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.FeatureManagement;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.HttpApi;
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
    typeof(ProApplicationContractsModule),
    typeof(AbpPermissionManagementHttpApiModule),
    typeof(AbpFeatureManagementHttpApiModule),
    typeof(AbpSettingManagementHttpApiModule))]

[DependsOn(typeof(AbpAccountPublicHttpApiModule))]
[DependsOn(typeof(AbpAccountAdminHttpApiModule))]
[DependsOn(typeof(AbpAuditLoggingHttpApiModule))]

// [DependsOn(typeof(AbpBasicDataHttpApiModule))]
[DependsOn(typeof(AbpChatHttpApiModule))]
[DependsOn(typeof(CmsKitProHttpApiModule))]
[DependsOn(typeof(AbpFileManagementHttpApiModule))]
[DependsOn(typeof(AbpFormsHttpApiModule))]
[DependsOn(typeof(AbpGdprHttpApiModule))]
[DependsOn(typeof(AbpIdentityProHttpApiModule))]
[DependsOn(typeof(AbpIdentityServerProHttpApiModule))]

// [DependsOn(typeof(AbpIotHttpApiModule))]
[DependsOn(typeof(AbpLanguageManagementHttpApiModule))]

// LeptonTheme
[DependsOn(typeof(AbpNotificationHttpApiModule))]
[DependsOn(typeof(AbpOpenIddictProHttpApiModule))]
[DependsOn(typeof(AbpPaymentAdminHttpApiModule))]
[DependsOn(typeof(AbpPaymentHttpApiModule))]
[DependsOn(typeof(AbpQuartzHttpApiModule))]
[DependsOn(typeof(AbpSaasHttpApiModule))]
[DependsOn(typeof(AbpTextTemplateManagementHttpApiModule))]
[DependsOn(typeof(AbpVersionManagementHttpApiModule))]
public class ProHttpApiModule : AbpModule
{
}
