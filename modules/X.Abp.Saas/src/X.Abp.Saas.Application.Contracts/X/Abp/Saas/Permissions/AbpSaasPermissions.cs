// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Reflection;

namespace X.Abp.Saas.Permissions;

public class AbpSaasPermissions
{
    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(AbpSaasPermissions));
    }

    public const string GroupName = "Saas";

    public static class Tenants
    {
        public const string Default = GroupName + ".Tenants";

        public const string Create = Default + ".Create";

        public const string Update = Default + ".Update";

        public const string Delete = Default + ".Delete";

        public const string ManageFeatures = Default + ".ManageFeatures";

        public const string ManageConnectionStrings = Default + ".ManageConnectionStrings";

        public const string ViewChangeHistory = "AuditLogging.ViewChangeHistory:Saas.Tenant";

        public const string Impersonation = Default + ".Impersonation";

        public const string SetPassword = Default + ".SetPassword";
    }

    public static class Editions
    {
        public const string Default = GroupName + ".Editions";

        public const string Create = Default + ".Create";

        public const string Update = Default + ".Update";

        public const string Delete = Default + ".Delete";

        public const string ManageFeatures = Default + ".ManageFeatures";

        public const string ViewChangeHistory = "AuditLogging.ViewChangeHistory:Saas.Edition";
    }
}
