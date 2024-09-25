// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Reflection;

namespace X.Abp.AuditLogging.Permissions;

public class AbpAuditLoggingPermissions
{
    public const string GroupName = "AuditLogging";

    public class AuditLogs
    {
        public const string Default = GroupName + ".AuditLogs";
        public const string SettingManagement = Default + ".SettingManagement";
    }

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(AbpAuditLoggingPermissions));
    }
}
