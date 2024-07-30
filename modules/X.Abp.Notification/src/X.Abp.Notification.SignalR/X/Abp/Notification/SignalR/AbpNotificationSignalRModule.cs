// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.AspNetCore.SignalR;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

using X.Abp.Notification.Localization;
using X.Abp.Notification.SignalR.RealTime;

namespace X.Abp.Notification.SignalR
{
    [DependsOn(
        typeof(AbpNotificationModule),
        typeof(AbpAspNetCoreSignalRModule))]
    public class AbpNotificationSignalRModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddSignalR();
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<AbpNotificationSignalRModule>();
            });

            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Get<AbpNotificationResource>()
                    .AddVirtualJson("/X/Abp/Notification/SignalR/Localization/Resources");
            });

            Configure<AbpExceptionLocalizationOptions>(options =>
            {
                options.MapCodeNamespace("X.Abp.Notification", typeof(AbpNotificationResource));
            });

            Configure<AbpNotificationOptions>(options =>
            {
                options.Notifiers.Add<SignalRRealTimeNotifier>();
            });

            Configure<AbpSignalROptions>(options =>
            {
                options.Hubs.Add(
                    new HubConfig(
                        typeof(NotificationHub), // Hub type
                        "/notification-hub", // Hub route (URL)
                        hubOptions =>
                        {
                            // Additional options
                            hubOptions.LongPolling.PollTimeout = TimeSpan.FromSeconds(30);
                        }));
            });

            /*
            context.Services.Configure<AbpEndpointRouterOptions>(options =>
            {
                options.EndpointConfigureActions.Add(endpointContext =>
                {
                    endpointContext.Endpoints.MapHub<NotificationHub>("/notification-hub", options =>
                    {
                        options.LongPolling.PollTimeout = TimeSpan.FromSeconds(30);
                    });
                });
            });
            */
        }
    }
}
