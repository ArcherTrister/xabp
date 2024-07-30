// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

using Volo.Abp.Localization;

using X.Abp.Notification.RealTime;

namespace X.Abp.Notification.SignalR.RealTime
{
    /// <summary>
    /// Implements <see cref="IRealTimeNotifier"/> to send notifications via SignalR.
    /// </summary>
    public class SignalRRealTimeNotifier : RealTimeNotifier
    {
        public const string NotifierName = "SignalR";

        public override string Name => NotifierName;

        public override ILocalizableString DisplayName => L("SignalRNotifier");

        public override bool UseOnlyIfRequestedAsTarget => false;

        protected IHubContext<NotificationHub> NotificationHub { get; }

        protected IOnlineClientManager OnlineClientManager { get; }

        protected ILogger<SignalRRealTimeNotifier> Logger { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SignalRRealTimeNotifier"/> class.
        /// </summary>
        public SignalRRealTimeNotifier(
            IHubContext<NotificationHub> notificationHub,
            IOnlineClientManager onlineClientManager,
            ILogger<SignalRRealTimeNotifier> logger)
        {
            NotificationHub = notificationHub;
            OnlineClientManager = onlineClientManager;
            Logger = logger;
        }

        /// <inheritdoc/>
        public override async Task SendNotificationsAsync(UserNotificationInfo[] userNotifications)
        {
            foreach (var userNotification in userNotifications)
            {
                try
                {
                    var onlineClients = await OnlineClientManager.GetAllByUserIdAsync(userNotification);
                    foreach (var onlineClient in onlineClients)
                    {
                        var signalRClient = NotificationHub.Clients.Client(onlineClient.ConnectionId);
                        if (signalRClient == null)
                        {
                            Logger.LogDebug("Can not get user " + userNotification.ToUserIdentifier() + " with connectionId " + onlineClient.ConnectionId + " from SignalR hub!");
                            continue;
                        }

                        await signalRClient.SendAsync("receiveNotification", userNotification);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogWarning("Could not send notification to user: " + userNotification.ToUserIdentifier());
                    Logger.LogWarning(ex.ToString(), ex);
                }
            }
        }
    }
}
