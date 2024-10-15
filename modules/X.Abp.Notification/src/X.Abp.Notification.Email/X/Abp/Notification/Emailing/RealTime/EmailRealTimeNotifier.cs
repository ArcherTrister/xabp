// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Volo.Abp.Emailing;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

using X.Abp.Notification.RealTime;

using IIdentityUserRepository = X.Abp.Identity.IIdentityUserRepository;

namespace X.Abp.Notification.Email.RealTime
{
    /// <summary>
    /// Implements <see cref="IRealTimeNotifier"/> to send notifications via Email.
    /// </summary>
    public class EmailRealTimeNotifier : RealTimeNotifier
    {
        public const string NotifierName = "Email";

        public override string Name => NotifierName;

        public override ILocalizableString DisplayName => L("EmailNotifier");

        public override bool UseOnlyIfRequestedAsTarget => false;

        protected IEmailSender EmailSender { get; }

        protected IIdentityUserRepository UserRepository { get; }

        protected ICurrentTenant CurrentTenant { get; }

        protected ILogger<EmailRealTimeNotifier> Logger { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailRealTimeNotifier"/> class.
        /// </summary>
        public EmailRealTimeNotifier(IEmailSender emailSender,
            IIdentityUserRepository userRepository,
            ICurrentTenant currentTenant,
            ILogger<EmailRealTimeNotifier> logger)
        {
            EmailSender = emailSender;
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
                            if (userNotification.Notification.Data is SubjectMessageNotificationData data)
                            {
                                var user = usersToNotify.FirstOrDefault(x => x.Id == userNotification.UserId);
                                if (user == null)
                                {
                                    Logger.LogDebug("Can not send email to user: " + userNotification.UserId + ". User does not exists!");
                                    continue;
                                }

                                if (user.Email.IsNullOrWhiteSpace())
                                {
                                    Logger.LogInformation("Can not send email to user: " + user.Name + ". User's email is empty!");
                                    continue;
                                }

                                await EmailSender.SendAsync(user.Email, data.Subject, data.Message);
                            }
                            else
                            {
                                Logger.LogDebug("Subject property or Message property is not found in notification data. Notification cannot be sent.");
                                continue;
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
    }
}
