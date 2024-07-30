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
using X.Abp.Gdpr;
using X.Abp.Identity;
using X.Abp.IdentityServer;
using X.Abp.LanguageManagement;
using X.Abp.Saas;
using X.Abp.TextTemplateManagement;

namespace MyCompanyName.MyProjectName;

[DependsOn(
    typeof(MyProjectNameApplicationContractsModule),
    typeof(AbpIdentityProHttpApiClientModule),
    typeof(AbpPermissionManagementHttpApiClientModule),
    typeof(AbpFeatureManagementHttpApiClientModule),
    typeof(AbpSettingManagementHttpApiClientModule),
    typeof(AbpSaasHttpApiClientModule),
    typeof(AbpAuditLoggingHttpApiClientModule),
    typeof(AbpIdentityServerProHttpApiClientModule),
    typeof(AbpAccountAdminHttpApiClientModule),
    typeof(AbpAccountPublicHttpApiClientModule),
    typeof(AbpLanguageManagementHttpApiClientModule),
    typeof(AbpGdprHttpApiClientModule),
    typeof(AbpTextTemplateManagementHttpApiClientModule)
)]
public class MyProjectNameHttpApiClientModule : AbpModule
{
    public const string RemoteServiceName = "Default";

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddHttpClientProxies(
            typeof(MyProjectNameApplicationContractsModule).Assembly,
            RemoteServiceName
        );

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<MyProjectNameHttpApiClientModule>();
        });
    }
}
