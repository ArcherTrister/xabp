// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;

namespace X.Abp.LeptonTheme.Management
{
    [DependsOn(
        typeof(AbpLeptonThemeManagementDomainSharedModule),
        typeof(AbpDddApplicationContractsModule),
        typeof(AbpAuthorizationAbstractionsModule))]
    public class AbpLeptonThemeManagementApplicationContractsModule : AbpModule
    {
    }
}
