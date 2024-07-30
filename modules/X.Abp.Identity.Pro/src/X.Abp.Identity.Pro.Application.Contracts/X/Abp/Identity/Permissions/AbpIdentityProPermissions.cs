// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Reflection;

namespace X.Abp.Identity.Permissions;

public static class AbpIdentityProPermissions
{
    public const string GroupName = "AbpIdentity";

    public const string SettingManagement = GroupName + ".SettingManagement";

    public static class Roles
    {
        public const string Default = GroupName + ".Roles";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
        public const string ManagePermissions = Default + ".ManagePermissions";
        public const string ViewChangeHistory = "AuditLogging.ViewChangeHistory:Volo.Abp.Identity.IdentityRole";
    }

    public static class Users
    {
        public const string Default = GroupName + ".Users";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
        public const string ManagePermissions = Default + ".ManagePermissions";
        public const string ViewChangeHistory = "AuditLogging.ViewChangeHistory:Volo.Abp.Identity.IdentityUser";
        public const string Impersonation = Default + ".Impersonation";
        public const string Import = Default + ".Import";
    }

    public static class ClaimTypes
    {
        public const string Default = GroupName + ".ClaimTypes";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static class UserLookup
    {
        public const string Default = GroupName + ".UserLookup";
    }

    public static class OrganizationUnits
    {
        public const string Default = GroupName + ".OrganizationUnits";
        public const string ManageOU = Default + ".ManageOU";
        public const string ManageRoles = Default + ".ManageRoles";
        public const string ManageUsers = Default + ".ManageMembers";
    }

    public static class SecurityLogs
    {
        public const string Default = GroupName + ".SecurityLogs";
    }

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(AbpIdentityProPermissions));
    }
}
