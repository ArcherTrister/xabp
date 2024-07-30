// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Application;
using Volo.Abp.Emailing;
using Volo.Abp.Json;
using Volo.Abp.Modularity;
using Volo.Abp.SettingManagement;
using Volo.Abp.UI.Navigation;

using X.Abp.Identity;

namespace X.Abp.Account;

[DependsOn(
    typeof(AbpAccountSharedApplicationContractsModule),
    typeof(AbpEmailingModule),
    typeof(AbpIdentityProDomainModule),
    typeof(AbpSettingManagementDomainModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpUiNavigationModule),
    typeof(AbpJsonModule))]
public class AbpAccountSharedApplicationModule : AbpModule
{
}
