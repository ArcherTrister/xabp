// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Sms;

using X.Abp.Identity;
using X.Abp.Notification.RealTime;

namespace X.Abp.Notification.Sms.RealTime
{
    /// <summary>
    /// Implements <see cref="IRealTimeNotifier"/> to send notifications via Sms.
    /// </summary>
    public class SmsRealTimeNotifier : RealTimeNotifier
    {
        public const string NotifierName = "Sms";

        public override string Name => NotifierName;

        public override ILocalizableString DisplayName => L("SmsNotifier");

        public override bool UseOnlyIfRequestedAsTarget => false;

        protected ISmsSender SmsSender { get; }

        protected SmsRealTimeNotifierOptions SmsRealTimeNotifierOptions { get; }

        protected IIdentityUserRepository UserRepository { get; }

        protected ICurrentTenant CurrentTenant { get; }

        protected ILogger<SmsRealTimeNotifier> Logger { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SmsRealTimeNotifier"/> class.
        /// </summary>
        public SmsRealTimeNotifier(ISmsSender smsSender,
            IOptions<SmsRealTimeNotifierOptions> options,
            IIdentityUserRepository userRepository,
            ICurrentTenant currentTenant,
            ILogger<SmsRealTimeNotifier> logger)
        {
            SmsSender = smsSender;
            SmsRealTimeNotifierOptions = options.Value;
            UserRepository = userRepository;
            CurrentTenant = currentTenant;
            Logger = logger;
        }

        /// <inheritdoc/>
        public override async Task SendNotificationsAsync(UserNotificationInfo[] userNotifications)
        {
            var userNotificationsGroupedByTenant = userNotifications.GroupBy(un => un.TenantId);
            foreach (var userNotificationByTenant in userNotificationsGroupedByTenant)
            {
                using (CurrentTenant.Change(userNotificationByTenant.Key))
                {
                    // IExternalUserLookupServiceProvider
                    var allUserIds = userNotificationByTenant.ToList().Select(x => x.UserId).Distinct().ToList();
                    var usersToNotify = await UserRepository.GetListByIdsAsync(allUserIds);

                    foreach (var userNotification in userNotificationByTenant)
                    {
                        try
                        {
                            if (userNotification.Notification.Data is MessageNotificationData data)
                            {
                                var user = usersToNotify.FirstOrDefault(x => x.Id == userNotification.UserId);
                                if (user == null)
                                {
                                    Logger.LogDebug($"Can not send sms to user: {userNotification.UserId}. User does not exists!");
                                    continue;
                                }

                                if (user.PhoneNumber.IsNullOrWhiteSpace())
                                {
                                    Logger.LogInformation($"Can not send sms to user: {user.Name}. User's phoneNumber is empty!");
                                    continue;
                                }

                                SmsMessage smsMessage = new SmsMessage(user.PhoneNumber, data.Message);

                                foreach (var item in SmsRealTimeNotifierOptions.SmsPlatformConfigs)
                                {
                                    Logger.LogDebug($"SmsConfig => key: {item.Key},value: {item.Value}");
                                    smsMessage.Properties.Add(item.Key, item.Value);
                                }

                                await SmsSender.SendAsync(smsMessage);
                            }
                            else
                            {
                                Logger.LogDebug("Message property is not found in notification data. Notification cannot be sent.");
                                continue;
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.LogWarning($"Could not send notification to user: {userNotification.ToUserIdentifier()}");
                            Logger.LogWarning(ex.ToString(), ex);
                        }
                    }
                }
            }
        }
    }
}
