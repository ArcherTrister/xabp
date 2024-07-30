// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.SettingManagement;

namespace X.Abp.LeptonTheme.Management
{
    [DependsOn(
        typeof(AbpLeptonThemeManagementDomainModule),
        typeof(AbpLeptonThemeManagementApplicationContractsModule),
        typeof(AbpAutoMapperModule),
        typeof(AbpSettingManagementDomainModule),
        typeof(AbpDddApplicationModule))]
    public class AbpLeptonThemeManagementApplicationModule : AbpModule
    {
    }
}
