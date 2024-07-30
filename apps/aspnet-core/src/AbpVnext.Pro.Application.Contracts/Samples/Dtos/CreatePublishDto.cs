//// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
//// See https://github.com/ArcherTrister/xabp
//// for more information concerning the license and the contributors participating to this project.

//using System;

//using X.Abp.Notification;

//namespace AbpVnext.Pro.Samples.Dtos
//{
//    public class CreatePublishDto
//    {
//        /// <summary>
//        /// Unique notification name
//        /// </summary>
//        public string NotificationName { get; set; }

//        /// <summary>
//        /// Notification data (optional)
//        /// </summary>
//        public NotificationData Data { get; set; }

//        /// <summary>
//        /// The entity identifier if this notification is related to an entity
//        /// </summary>
//        public EntityIdentifier EntityIdentifier { get; set; }

//        /// <summary>
//        /// Notification severity
//        /// </summary>
//        public NotificationSeverity Severity { get; set; }

//        /// <summary>
//        /// Target user id(s).
//        /// Used to send notification to specific user(s) (without checking the subscription).
//        /// If this is null/empty, the notification is sent to subscribed users.
//        /// </summary>
//        public UserIdentifier[] UserIds { get; set; }

//        /// <summary>
//        /// Excluded user id(s).
//        /// This can be set to exclude some users while publishing notifications to subscribed users.
//        /// It's normally not set if UserIds is set.
//        /// </summary>
//        public UserIdentifier[] ExcludedUserIds { get; set; }

//        /// <summary>
//        /// Target tenant id(s).
//        /// Used to send notification to subscribed users of specific tenant(s).
//        /// This should not be set if UserIds is set.
//        /// NotificationPublisher.AllTenants can be passed to indicate all tenants.
//        /// But this can only work in a single database approach (all tenants are stored in host database).
//        /// If this is null, then it's automatically set to the current tenant on ICurrentTenant.Id.
//        /// </summary>
//        public Guid?[] TenantIds { get; set; }

//        /// <summary>
//        /// Which realtime notifiers should handle this notification. Given notifier must be added to the AbpNotificationOptions.Notifiers
//        /// </summary>
//        public string[] TargetNotifiers { get; set; }
//    }
//}
