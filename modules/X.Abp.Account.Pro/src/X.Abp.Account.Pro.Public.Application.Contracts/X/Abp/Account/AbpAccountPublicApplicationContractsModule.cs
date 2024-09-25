// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp.Emailing;
using Volo.Abp.Modularity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.ObjectExtending.Modularity;
using Volo.Abp.Threading;
using Volo.Abp.VirtualFileSystem;

using X.Abp.Account.Dtos;

namespace X.Abp.Account;

[DependsOn(
    typeof(AbpEmailingModule),
    typeof(AbpAccountSharedApplicationContractsModule))]
public class AbpAccountPublicApplicationContractsModule : AbpModule
{
    private static readonly OneTimeRunner OneTimeRunner = new OneTimeRunner();

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
            options.FileSets.AddEmbedded<AbpAccountPublicApplicationContractsModule>());
    }

    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        OneTimeRunner.Run(
            () =>
                ModuleExtensionConfigurationHelper.ApplyEntityConfigurationToApi(
                    IdentityModuleExtensionConsts.ModuleName,
                    IdentityModuleExtensionConsts.EntityNames.User,
                    getApiTypes: new Type[] { typeof(ProfileDto) },
                    updateApiTypes: new Type[] { typeof(UpdateProfileDto) }));
    }
}
