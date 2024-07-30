// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Volo.Abp.DependencyInjection;
using Volo.Abp.Localization;

using X.Abp.Notification.Localization;

namespace X.Abp.Notification.RealTime
{
    public abstract class RealTimeNotifier : IRealTimeNotifier, ITransientDependency
    {
        public abstract string Name { get; }

        public abstract ILocalizableString DisplayName { get; }

        public abstract bool UseOnlyIfRequestedAsTarget { get; }

        public abstract Task SendNotificationsAsync(UserNotificationInfo[] userNotifications);

        protected static LocalizableString L(string name)
        {
            return LocalizableString.Create<AbpNotificationResource>(name);
        }
    }
}
