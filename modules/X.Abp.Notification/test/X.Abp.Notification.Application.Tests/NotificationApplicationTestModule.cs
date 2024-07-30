using Volo.Abp.Modularity;

namespace X.Abp.Notification;

[DependsOn(
    typeof(AbpNotificationApplicationModule),
    typeof(NotificationDomainTestModule)
    )]
public class NotificationApplicationTestModule : AbpModule
{

}
