// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Reflection;

namespace X.Abp.VersionManagement.Permissions;

public class AbpVersionManagementPermissions
{
    public const string GroupName = "VersionManagement";

    /// <summary>
    /// 版本管理
    /// </summary>
    public static class AppEditions
    {
        public const string Default = GroupName + ".AppEditions";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(AbpVersionManagementPermissions));
    }
}
