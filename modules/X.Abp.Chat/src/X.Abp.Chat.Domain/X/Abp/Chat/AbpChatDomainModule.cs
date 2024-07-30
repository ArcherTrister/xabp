// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.Domain;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.SettingManagement;
using Volo.Abp.Users;

using X.Abp.Chat.Localization;

namespace X.Abp.Chat;

[DependsOn(
    typeof(AbpChatDomainSharedModule),
    typeof(AbpDddDomainModule),
    typeof(AbpSettingManagementDomainModule),
    typeof(AbpUsersDomainModule))]
public class AbpChatDomainModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.Configure<AbpExceptionLocalizationOptions>(options =>
        {
            options.MapCodeNamespace("X.Abp.Chat", typeof(AbpChatResource));
        });
    }
}
