// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace AbpVnext.Pro.Permissions;

public static class ProPermissions
{
    public const string GroupName = "Pro";

    public static class Dashboard
    {
        public const string DashboardGroup = GroupName + ".Dashboard";
        public const string Host = DashboardGroup + ".Host";
        public const string Tenant = DashboardGroup + ".Tenant";
    }

    // Add your own permission names. Example:
    // public const string MyPermission1 = GroupName + ".MyPermission1";
}
