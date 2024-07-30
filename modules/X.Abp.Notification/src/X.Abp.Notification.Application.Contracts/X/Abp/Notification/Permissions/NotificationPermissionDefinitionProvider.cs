// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

using X.Abp.Notification.Localization;

namespace X.Abp.Notification.Permissions;

public class NotificationPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        /*
        var myGroup = context.AddGroup(NotificationPermissions.GroupName, L("Permission:Notification"));
        var notificationPermission = myGroup.AddPermission(NotificationPermissions.Notifications.Default, L("Permission:Notifications"))
            .RequireFeatures(NotificationFeatures.Enable);
        notificationPermission.AddChild(NotificationPermissions.Notifications.Create, L("Permission:Create"))
            .RequireFeatures(NotificationFeatures.Enable);
        notificationPermission.AddChild(NotificationPermissions.Notifications.Edit, L("Permission:Edit"))
            .RequireFeatures(NotificationFeatures.Enable);

        var userNotificationPermission = myGroup.AddPermission(NotificationPermissions.UserNotifications.Default, L("Permission:UserNotifications"))
            .RequireFeatures(NotificationFeatures.Enable);
        userNotificationPermission.AddChild(NotificationPermissions.UserNotifications.Edit, L("Permission:Edit"))
            .RequireFeatures(NotificationFeatures.Enable);
        userNotificationPermission.AddChild(NotificationPermissions.UserNotifications.Delete, L("Permission:Delete"))
           .RequireFeatures(NotificationFeatures.Enable);
        */
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<AbpNotificationResource>(name);
    }
}
