// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.FeatureManagement;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.VirtualFileSystem;

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
    typeof(AbpIdentityProHttpApiClientModule),
    typeof(AbpPermissionManagementHttpApiClientModule),
    typeof(AbpFeatureManagementHttpApiClientModule),
    typeof(AbpSettingManagementHttpApiClientModule),
    typeof(AbpSaasHttpApiClientModule),
    typeof(AbpAuditLoggingHttpApiClientModule),
    typeof(AbpOpenIddictProHttpApiClientModule),
    typeof(AbpIdentityServerProHttpApiClientModule),
    typeof(AbpAccountAdminHttpApiClientModule),
    typeof(AbpAccountPublicHttpApiClientModule),
    typeof(AbpLanguageManagementHttpApiClientModule),
    typeof(AbpGdprHttpApiClientModule),
    typeof(AbpTextTemplateManagementHttpApiClientModule))]
//[DependsOn(typeof(AbpBasicDataHttpApiClientModule))]
[DependsOn(typeof(AbpNotificationHttpApiClientModule))]
//[DependsOn(typeof(AbpIotHttpApiClientModule))]
[DependsOn(typeof(AbpOpenIddictProHttpApiClientModule))]
[DependsOn(typeof(AbpQuartzHttpApiClientModule))]
[DependsOn(typeof(AbpChatHttpApiClientModule))]
[DependsOn(typeof(AbpFileManagementHttpApiClientModule))]
[DependsOn(typeof(AbpFormsHttpApiClientModule))]
//[DependsOn(typeof(CmsKitProAdminHttpApiClientModule))]
[DependsOn(typeof(CmsKitProHttpApiClientModule))]
//[DependsOn(typeof(CmsKitProPublicHttpApiClientModule))]
[DependsOn(typeof(AbpPaymentAdminHttpApiClientModule))]
[DependsOn(typeof(AbpPaymentHttpApiClientModule))]
[DependsOn(typeof(AbpVersionManagementHttpApiClientModule))]
public class ProHttpApiClientModule : AbpModule
{
    public const string RemoteServiceName = "Default";

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddHttpClientProxies(
            typeof(ProApplicationContractsModule).Assembly,
            RemoteServiceName
        );

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<ProHttpApiClientModule>();
        });
    }
}
