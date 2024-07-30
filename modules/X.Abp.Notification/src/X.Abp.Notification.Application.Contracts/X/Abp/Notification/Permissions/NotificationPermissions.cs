// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Reflection;

namespace X.Abp.Notification.Permissions;

public class NotificationPermissions
{
    public const string GroupName = "Notification";

    /*
    public static class Notifications
    {
        public const string Default = GroupName + ".Notifications";
        public const string Create = GroupName + ".Create";
        public const string Edit = GroupName + ".Edit";
    }

    public static class UserNotifications
    {
        public const string Default = GroupName + ".UserNotifications";
        public const string Delete = GroupName + ".Delete";
        public const string Edit = GroupName + ".Edit";
    }
    */

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(NotificationPermissions));
    }
}
