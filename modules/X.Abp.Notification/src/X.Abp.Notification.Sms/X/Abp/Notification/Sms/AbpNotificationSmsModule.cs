// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.Sms;
using Volo.Abp.VirtualFileSystem;

using X.Abp.Identity;
using X.Abp.Notification.Localization;
using X.Abp.Notification.Sms.RealTime;

namespace X.Abp.Notification.Sms
{
    [DependsOn(
        typeof(AbpIdentityProDomainModule),
        typeof(AbpSmsModule),
        typeof(AbpNotificationModule))]
    public class AbpNotificationSmsModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<AbpNotificationSmsModule>();
            });

            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Get<AbpNotificationResource>()
                    .AddVirtualJson("/X/Abp/Notification/Sms/Localization/Resources");
            });

            Configure<AbpExceptionLocalizationOptions>(options =>
            {
                options.MapCodeNamespace("X.Abp.Notification", typeof(AbpNotificationResource));
            });

            var smsRealTimeNotifierOptions = context.Services.GetPreConfigureActions<SmsRealTimeNotifierOptions>();
            Configure<SmsRealTimeNotifierOptions>(smsRealTimeNotifierOptions.Configure);

            Configure<AbpNotificationOptions>(options =>
            {
                options.Notifiers.Add<SmsRealTimeNotifier>();
            });
        }
    }
}
