// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Reflection;

namespace X.Abp.LanguageManagement.Permissions;

public class AbpLanguageManagementPermissions
{
    public const string GroupName = "LanguageManagement";

    public class LanguageTexts
    {
        public const string Default = GroupName + ".LanguageTexts";
        public const string Edit = Default + ".Edit";
    }

    public class Languages
    {
        public const string Default = GroupName + ".Languages";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string ChangeDefault = Default + ".ChangeDefault";
        public const string Delete = Default + ".Delete";
    }

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(AbpLanguageManagementPermissions));
    }
}
