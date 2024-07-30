// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MyCompanyName.MyProjectName.Localization;

using Volo.Abp.AuditLogging;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.BlobStoring.Database;
using Volo.Abp.FeatureManagement;
using Volo.Abp.GlobalFeatures;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

using X.Abp.Gdpr;
using X.Abp.Identity;
using X.Abp.OpenIddict;
using X.Abp.LanguageManagement;
using X.Abp.Saas;
using X.Abp.TextTemplateManagement;

namespace MyCompanyName.MyProjectName;

[DependsOn(
    typeof(AbpAuditLoggingDomainSharedModule),
    typeof(AbpBackgroundJobsDomainSharedModule),
    typeof(AbpFeatureManagementDomainSharedModule),
    typeof(AbpIdentityProDomainSharedModule),
    typeof(AbpOpenIddictProDomainSharedModule),
    typeof(AbpPermissionManagementDomainSharedModule),
    typeof(AbpSettingManagementDomainSharedModule),
    typeof(AbpLanguageManagementDomainSharedModule),
    typeof(AbpSaasDomainSharedModule),
    typeof(AbpTextTemplateManagementDomainSharedModule),
    typeof(AbpGdprDomainSharedModule),
    typeof(AbpGlobalFeaturesModule),
    typeof(BlobStoringDatabaseDomainSharedModule)
    )]
public class MyProjectNameDomainSharedModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        MyProjectNameGlobalFeatureConfigurator.Configure();
        MyProjectNameModuleExtensionConfigurator.Configure();
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<MyProjectNameDomainSharedModule>();
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Add<MyProjectNameResource>("en")
                .AddVirtualJson("/Localization/MyProjectName");

            options.DefaultResourceType = typeof(MyProjectNameResource);
        });

        Configure<AbpExceptionLocalizationOptions>(options =>
        {
            options.MapCodeNamespace("MyCompanyName.MyProjectName", typeof(MyProjectNameResource));
        });
    }
}
