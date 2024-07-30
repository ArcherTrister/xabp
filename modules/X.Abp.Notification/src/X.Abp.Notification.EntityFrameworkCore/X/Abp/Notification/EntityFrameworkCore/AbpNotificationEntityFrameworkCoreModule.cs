// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace X.Abp.Notification.EntityFrameworkCore;

[DependsOn(
    typeof(AbpNotificationDomainModule),
    typeof(AbpEntityFrameworkCoreModule))]
public class AbpNotificationEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<NotificationDbContext>(options =>
        {
            /* Add custom repositories here. Example:
             * options.AddRepository<Question, EfCoreQuestionRepository>();
             */
            options.AddDefaultRepositories<INotificationDbContext>();

            options.AddRepository<NotificationGroupDefinitionRecord, EfCoreNotificationGroupDefinitionRecordRepository>();
            options.AddRepository<NotificationDefinitionRecord, EfCoreNotificationDefinitionRecordRepository>();
        });
    }
}
