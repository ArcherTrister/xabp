// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Volo.Abp.FeatureManagement;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.HttpApi;
using Volo.Abp.SettingManagement;

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
    typeof(AbpIdentityProHttpApiModule),
    typeof(AbpPermissionManagementHttpApiModule),
    typeof(AbpFeatureManagementHttpApiModule),
    typeof(AbpSettingManagementHttpApiModule),
    typeof(AbpAuditLoggingHttpApiModule),
    typeof(AbpIdentityServerProHttpApiModule),
    typeof(AbpAccountAdminHttpApiModule),
    typeof(AbpLanguageManagementHttpApiModule),
    typeof(AbpSaasHttpApiModule),
    typeof(AbpGdprHttpApiModule),
    typeof(AbpAccountPublicHttpApiModule),
    typeof(AbpTextTemplateManagementHttpApiModule)
    )]
public class MyProjectNameHttpApiModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        //Configure<AbpLocalizationOptions>(options =>
        //{
        //    options.Resources
        //        .Get<MyProjectNameResource>()
        //        .AddBaseTypes(typeof(AbpUiResource));
        //});
    }
}
