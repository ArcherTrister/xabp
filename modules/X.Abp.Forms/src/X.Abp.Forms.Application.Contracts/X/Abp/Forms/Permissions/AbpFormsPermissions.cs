// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Reflection;

namespace X.Abp.Forms.Permissions;

public class AbpFormsPermissions
{
    public const string GroupName = "Forms";

    public static class Forms
    {
        public const string Default = GroupName + ".Form";
        public const string Delete = Default + ".Delete";
    }

    public static class Response
    {
        public const string Default = GroupName + ".Response";
        public const string Delete = Default + ".Delete";
    }

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(AbpFormsPermissions));
    }
}
