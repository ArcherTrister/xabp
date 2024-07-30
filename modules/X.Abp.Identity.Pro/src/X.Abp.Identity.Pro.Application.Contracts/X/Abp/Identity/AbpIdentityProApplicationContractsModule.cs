// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Authorization;
using Volo.Abp.Modularity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.ObjectExtending.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Threading;
using Volo.Abp.Users;

namespace X.Abp.Identity;

[DependsOn(
    typeof(AbpIdentityProDomainSharedModule),
    typeof(AbpUsersAbstractionModule),
    typeof(AbpPermissionManagementApplicationContractsModule),
    typeof(AbpAuthorizationModule))]
public class AbpIdentityProApplicationContractsModule : AbpModule
{
    private static readonly OneTimeRunner OneTimeRunner = new();

    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        OneTimeRunner.Run(() =>
        {
            ModuleExtensionConfigurationHelper
                .ApplyEntityConfigurationToApi(
                    IdentityModuleExtensionConsts.ModuleName,
                    IdentityModuleExtensionConsts.EntityNames.User,
                    getApiTypes: new[] { typeof(IdentityUserDto) },
                    createApiTypes: new[] { typeof(IdentityUserCreateDto) },
                    updateApiTypes: new[] { typeof(IdentityUserUpdateDto) });

            ModuleExtensionConfigurationHelper
                .ApplyEntityConfigurationToApi(
                    IdentityModuleExtensionConsts.ModuleName,
                    IdentityModuleExtensionConsts.EntityNames.Role,
                    getApiTypes: new[] { typeof(IdentityRoleDto) },
                    createApiTypes: new[] { typeof(IdentityRoleCreateDto) },
                    updateApiTypes: new[] { typeof(IdentityRoleUpdateDto) });

            ModuleExtensionConfigurationHelper
                .ApplyEntityConfigurationToApi(
                    IdentityModuleExtensionConsts.ModuleName,
                    IdentityModuleExtensionConsts.EntityNames.ClaimType,
                    getApiTypes: new[] { typeof(ClaimTypeDto) },
                    createApiTypes: new[] { typeof(CreateClaimTypeDto) },
                    updateApiTypes: new[] { typeof(UpdateClaimTypeDto) });

            ModuleExtensionConfigurationHelper
                .ApplyEntityConfigurationToApi(
                    IdentityModuleExtensionConsts.ModuleName,
                    IdentityModuleExtensionConsts.EntityNames.OrganizationUnit,
                    getApiTypes: new[] { typeof(OrganizationUnitDto), typeof(OrganizationUnitWithDetailsDto) },
                    createApiTypes: new[] { typeof(OrganizationUnitCreateDto) },
                    updateApiTypes: new[] { typeof(OrganizationUnitUpdateDto) });
        });
    }
}
