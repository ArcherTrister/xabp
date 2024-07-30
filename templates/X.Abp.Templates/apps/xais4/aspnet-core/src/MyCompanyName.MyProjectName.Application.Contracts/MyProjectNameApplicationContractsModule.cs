// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Volo.Abp.FeatureManagement;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
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
    typeof(MyProjectNameDomainSharedModule),
    typeof(AbpFeatureManagementApplicationContractsModule),
    typeof(AbpIdentityProApplicationContractsModule),
    typeof(AbpPermissionManagementApplicationContractsModule),
    typeof(AbpSettingManagementApplicationContractsModule),
    typeof(AbpSaasApplicationContractsModule),
    typeof(AbpAuditLoggingApplicationContractsModule),
    typeof(AbpIdentityServerProApplicationContractsModule),
    typeof(AbpAccountPublicApplicationContractsModule),
    typeof(AbpAccountAdminApplicationContractsModule),
    typeof(AbpLanguageManagementApplicationContractsModule),
    typeof(AbpGdprApplicationContractsModule),
    typeof(AbpTextTemplateManagementApplicationContractsModule)
)]
public class MyProjectNameApplicationContractsModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        MyProjectNameDtoExtensions.Configure();
    }
}
