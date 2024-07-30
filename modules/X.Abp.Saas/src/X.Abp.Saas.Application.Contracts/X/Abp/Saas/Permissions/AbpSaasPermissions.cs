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
        public const string Default = "Saas.Tenants";

        public const string Create = "Saas.Tenants.Create";

        public const string Update = "Saas.Tenants.Update";

        public const string Delete = "Saas.Tenants.Delete";

        public const string ManageFeatures = "Saas.Tenants.ManageFeatures";

        public const string ManageConnectionStrings = "Saas.Tenants.ManageConnectionStrings";

        public const string ViewChangeHistory = "AuditLogging.ViewChangeHistory:Saas.Tenant";

        public const string Impersonation = "Saas.Tenants.Impersonation";

        public const string SetPassword = "Saas.Tenants.SetPassword";
    }

    public static class Editions
    {
        public const string Default = "Saas.Editions";

        public const string Create = "Saas.Editions.Create";

        public const string Update = "Saas.Editions.Update";

        public const string Delete = "Saas.Editions.Delete";

        public const string ManageFeatures = "Saas.Editions.ManageFeatures";

        public const string ViewChangeHistory = "AuditLogging.ViewChangeHistory:Saas.Edition";
    }
}
