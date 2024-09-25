// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Volo.Abp.Json;
using Volo.Abp.Localization;

namespace X.Abp.Notification.RealTime
{
    public class NullRealTimeNotifier : RealTimeNotifier
    {
        public const string NotifierName = "Null";

        public override string Name => NotifierName;

        public override ILocalizableString DisplayName => L("NullNotifier");

        public override bool UseOnlyIfRequestedAsTarget => false;

        protected IJsonSerializer JsonSerializer { get; }

        protected ILogger<NullRealTimeNotifier> Logger { get; }

        public NullRealTimeNotifier(ILogger<NullRealTimeNotifier> logger)
        {
            Logger = logger;
        }

        public override Task SendNotificationsAsync(UserNotificationInfo[] userNotifications)
        {
            Logger.LogDebug(JsonSerializer.Serialize(userNotifications));
            return Task.CompletedTask;
        }
    }
}
