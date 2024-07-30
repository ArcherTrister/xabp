// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.ObjectExtending.Modularity;
using Volo.Abp.SettingManagement;
using Volo.Abp.Threading;

using X.Abp.LanguageManagement.Dto;
using X.Abp.ObjectExtending;

namespace X.Abp.LanguageManagement;

[DependsOn(
    typeof(AbpLanguageManagementDomainModule),
    typeof(AbpLanguageManagementApplicationContractsModule),
    typeof(AbpAutoMapperModule),
    typeof(AbpSettingManagementDomainModule),
    typeof(AbpDddApplicationModule))]
public class AbpLanguageManagementApplicationModule : AbpModule
{
    private static readonly OneTimeRunner OneTimeRunner = new();

    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        OneTimeRunner.Run(() =>
        {
            ModuleExtensionConfigurationHelper
                .ApplyEntityConfigurationToApi(
                    LanguageManagementModuleExtensionConsts.ModuleName,
                    LanguageManagementModuleExtensionConsts.EntityNames.Language,
                    getApiTypes: new[] { typeof(LanguageDto) },
                    createApiTypes: new[] { typeof(CreateLanguageDto) },
                    updateApiTypes: new[] { typeof(UpdateLanguageDto) });
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAutoMapperObjectMapper<AbpLanguageManagementApplicationModule>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddProfile<AbpLanguageManagementApplicationAutoMapperProfile>(validate: true);
        });
    }
}
