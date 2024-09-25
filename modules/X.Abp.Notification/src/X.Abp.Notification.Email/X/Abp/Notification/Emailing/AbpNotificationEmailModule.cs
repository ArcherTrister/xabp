// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Emailing;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

using X.Abp.Identity;
using X.Abp.Notification.Email.RealTime;
using X.Abp.Notification.Localization;

namespace X.Abp.Notification.Email
{
    [DependsOn(
        typeof(AbpIdentityProDomainModule),
        typeof(AbpEmailingModule),
        typeof(AbpNotificationAbstractionsModule))]
    public class AbpNotificationEmailModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<AbpNotificationEmailModule>();
            });

            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Get<AbpNotificationResource>()
                    .AddVirtualJson("/X/Abp/Notification/Emailing/Localization/Resources");
            });

            Configure<AbpExceptionLocalizationOptions>(options =>
            {
                options.MapCodeNamespace("X.Abp.Notification", typeof(AbpNotificationResource));
            });

            Configure<AbpNotificationOptions>(options =>
            {
                options.Notifiers.Add<EmailRealTimeNotifier>();
            });
        }
    }
}
