// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.ObjectExtending.Modularity;
using Volo.Abp.Threading;
using Volo.Abp.VirtualFileSystem;

using X.Abp.OpenIddict.Applications.Dtos;
using X.Abp.OpenIddict.Scopes.Dtos;

namespace X.Abp.OpenIddict;

[DependsOn(
typeof(AbpOpenIddictProDomainSharedModule),
typeof(AbpDddApplicationContractsModule),
typeof(AbpAuthorizationModule))]
public class AbpOpenIddictProApplicationContractsModule : AbpModule
{
    private static readonly OneTimeRunner OneTimeRunner = new();

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<AbpOpenIddictProApplicationContractsModule>();
        });
    }

    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        OneTimeRunner.Run(() =>
        {
            ModuleExtensionConfigurationHelper.ApplyEntityConfigurationToApi(
                OpenIddictModuleExtensionConsts.ModuleName,
                OpenIddictModuleExtensionConsts.EntityNames.Application,
                getApiTypes: new[] { typeof(ApplicationDto) },
                createApiTypes: new[] { typeof(CreateApplicationInput) },
                updateApiTypes: new[] { typeof(UpdateApplicationInput) });

            ModuleExtensionConfigurationHelper.ApplyEntityConfigurationToApi(
                OpenIddictModuleExtensionConsts.ModuleName,
                OpenIddictModuleExtensionConsts.EntityNames.Scope,
                new Type[] { typeof(ScopeDto) },
                new Type[] { typeof(CreateScopeInput) },
                new Type[] { typeof(UpdateScopeInput) });
        });
    }
}
