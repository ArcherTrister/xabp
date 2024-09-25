// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Reflection;

namespace X.Abp.Chat.Permission;

public class AbpChatPermissions
{
    public const string GroupName = "Chat";

    public const string Messaging = GroupName + ".Messaging";

    public const string Searching = GroupName + ".Searching";

    public const string SettingManagement = GroupName + ".SettingManagement";

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(AbpChatPermissions));
    }
}
