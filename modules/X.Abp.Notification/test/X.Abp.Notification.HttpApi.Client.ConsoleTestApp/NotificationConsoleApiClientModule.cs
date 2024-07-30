using Volo.Abp.Autofac;
using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace X.Abp.Notification;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpNotificationHttpApiClientModule),
    typeof(AbpHttpClientIdentityModelModule)
    )]
public class NotificationConsoleApiClientModule : AbpModule
{

}
