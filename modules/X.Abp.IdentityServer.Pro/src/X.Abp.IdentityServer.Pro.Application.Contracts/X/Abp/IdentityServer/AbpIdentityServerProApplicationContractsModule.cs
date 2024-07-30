// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.IdentityServer.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.ObjectExtending.Modularity;
using Volo.Abp.Threading;
using Volo.Abp.VirtualFileSystem;

using X.Abp.IdentityServer.ApiResource.Dtos;
using X.Abp.IdentityServer.Client.Dtos;
using X.Abp.IdentityServer.IdentityResource.Dtos;

namespace X.Abp.IdentityServer;

[DependsOn(
    typeof(AbpIdentityServerProDomainSharedModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationAbstractionsModule))]
public class AbpIdentityServerProApplicationContractsModule : AbpModule
{
    private static readonly OneTimeRunner OneTimeRunner = new();

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<AbpIdentityServerProApplicationContractsModule>();
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<AbpIdentityServerResource>()
                .AddVirtualJson("/X/Abp/IdentityServer/Localization/Resources");
        });
    }

    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        OneTimeRunner.Run(() =>
        {
            ModuleExtensionConfigurationHelper
                .ApplyEntityConfigurationToApi(
                    IdentityServerModuleExtensionConsts.ModuleName,
                    IdentityServerModuleExtensionConsts.EntityNames.Client,
                    getApiTypes: new[] { typeof(ClientWithDetailsDto) },
                    createApiTypes: new[] { typeof(CreateClientDto) },
                    updateApiTypes: new[] { typeof(UpdateClientDto) });

            ModuleExtensionConfigurationHelper
                .ApplyEntityConfigurationToApi(
                    IdentityServerModuleExtensionConsts.ModuleName,
                    IdentityServerModuleExtensionConsts.EntityNames.IdentityResource,
                    getApiTypes: new[] { typeof(IdentityResourceWithDetailsDto) },
                    createApiTypes: new[] { typeof(CreateIdentityResourceDto) },
                    updateApiTypes: new[] { typeof(UpdateIdentityResourceDto) });

            ModuleExtensionConfigurationHelper
                .ApplyEntityConfigurationToApi(
                    IdentityServerModuleExtensionConsts.ModuleName,
                    IdentityServerModuleExtensionConsts.EntityNames.ApiResource,
                    getApiTypes: new[] { typeof(ApiResourceWithDetailsDto) },
                    createApiTypes: new[] { typeof(CreateApiResourceDto) },
                    updateApiTypes: new[] { typeof(UpdateApiResourceDto) });
        });
    }
}
