// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.AutoMapper;
using Volo.Abp.Caching;
using Volo.Abp.Data;
using Volo.Abp.Domain;
using Volo.Abp.Domain.Entities.Events.Distributed;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Features;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectExtending.Modularity;
using Volo.Abp.Threading;
using Volo.Abp.VirtualFileSystem;

using X.Abp.ObjectExtending;
using X.Abp.Saas.Editions;
using X.Abp.Saas.Features;
using X.Abp.Saas.Localization;
using X.Abp.Saas.Tenants;

namespace X.Abp.Saas;

[DependsOn(typeof(AbpMultiTenancyModule),
    typeof(AbpSaasDomainSharedModule),
    typeof(AbpDataModule),
    typeof(AbpDddDomainModule),
    typeof(AbpAutoMapperModule),
    typeof(AbpFeatureManagementDomainModule),
    typeof(AbpCachingModule))]
public class AbpSaasDomainModule : AbpModule
{
    private static readonly OneTimeRunner OneTimeRunner = new();

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<FeatureManagementOptions>(options =>
        {
            options.ProviderPolicies[TenantFeatureValueProvider.ProviderName] = "Saas.Tenants.ManageFeatures";
            options.ProviderPolicies[EditionFeatureValueProvider.ProviderName] = "Saas.Editions.ManageFeatures";
            options.Providers.InsertBefore(r => r == typeof(TenantFeatureManagementProvider), typeof(TenantEditionFeatureValueProvider));
        });

        context.Services.AddAutoMapperObjectMapper<AbpSaasDomainModule>();

        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddProfile<AbpSaasDomainMappingProfile>(true);
        });

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<AbpSaasDomainModule>();
        });

        Configure<AbpDistributedEntityEventOptions>(options =>
        {
            options.EtoMappings.Add<Edition, EditionEto>();
            options.EtoMappings.Add<Tenant, TenantEto>();

            options.AutoEventSelectors.Add<Edition>();
            options.AutoEventSelectors.Add<Tenant>();
        });

        Configure<AbpExceptionLocalizationOptions>(options =>
        {
            options.MapCodeNamespace("X.Abp.Saas", typeof(SaasResource));
        });
    }

    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        OneTimeRunner.Run(() =>
        {
            ModuleExtensionConfigurationHelper.ApplyEntityConfigurationToEntity(
                SaasModuleExtensionConsts.ModuleName,
                SaasModuleExtensionConsts.EntityNames.Tenant,
                typeof(Tenant));
            ModuleExtensionConfigurationHelper.ApplyEntityConfigurationToEntity(
                SaasModuleExtensionConsts.ModuleName,
                SaasModuleExtensionConsts.EntityNames.Edition,
                typeof(Edition));
        });
    }
}
