// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.IdentityServer.Permissions;

public class AbpIdentityServerProPermissions
{
    public const string GroupName = "IdentityServer";

    public static class ApiScope
    {
        public const string Default = "IdentityServer.ApiScope";

        public const string Delete = "IdentityServer.ApiScope.Delete";

        public const string Update = "IdentityServer.ApiScope.Update";

        public const string Create = "IdentityServer.ApiScope.Create";

        public const string ViewChangeHistory = "AuditLogging.ViewChangeHistory:Volo.Abp.IdentityServer.ApiScopes.ApiScope";
    }

    public static class IdentityResource
    {
        public const string Default = "IdentityServer.IdentityResource";

        public const string Delete = "IdentityServer.IdentityResource.Delete";

        public const string Update = "IdentityServer.IdentityResource.Update";

        public const string Create = "IdentityServer.IdentityResource.Create";

        public const string ViewChangeHistory = "AuditLogging.ViewChangeHistory:Volo.Abp.IdentityServer.IdentityResources.IdentityResource";
    }

    public static class ApiResource
    {
        public const string Default = "IdentityServer.ApiResource";

        public const string Delete = "IdentityServer.ApiResource.Delete";

        public const string Update = "IdentityServer.ApiResource.Update";

        public const string Create = "IdentityServer.ApiResource.Create";

        public const string ViewChangeHistory = "AuditLogging.ViewChangeHistory:Volo.Abp.IdentityServer.ApiResources.ApiResource";
    }

    public static class Client
    {
        public const string Default = "IdentityServer.Client";

        public const string Delete = "IdentityServer.Client.Delete";

        public const string Update = "IdentityServer.Client.Update";

        public const string Create = "IdentityServer.Client.Create";

        public const string ManagePermissions = "IdentityServer.Client.ManagePermissions";

        public const string ViewChangeHistory = "AuditLogging.ViewChangeHistory:Volo.Abp.IdentityServer.Clients.Client";
    }
}
