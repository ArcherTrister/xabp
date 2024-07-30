// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.AutoMapper;
using Volo.Abp.EventBus;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;

namespace X.Abp.Identity;

[DependsOn(
    typeof(AbpIdentityProDomainModule),
    typeof(AbpIdentityProApplicationContractsModule),
    typeof(AbpAutoMapperModule),
    typeof(AbpEventBusModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpSettingManagementDomainModule))]
public class AbpIdentityProApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAutoMapperObjectMapper<AbpIdentityProApplicationModule>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddProfile<AbpIdentityProApplicationModuleAutoMapperProfile>();
        });
    }
}
