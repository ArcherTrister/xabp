// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Reflection;

namespace X.Abp.FileManagement.Permissions;

public static class AbpFileManagementPermissions
{
    public const string GroupName = "FileManagement";

    public static class DirectoryDescriptor
    {
        public const string Default = GroupName + ".DirectoryDescriptor";

        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static class FileDescriptor
    {
        public const string Default = GroupName + ".FileDescriptor";

        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(AbpFileManagementPermissions));
    }
}
