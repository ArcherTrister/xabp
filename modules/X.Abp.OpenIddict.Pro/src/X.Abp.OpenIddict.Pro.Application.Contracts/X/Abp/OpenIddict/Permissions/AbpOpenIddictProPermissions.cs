// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Reflection;

namespace X.Abp.OpenIddict.Permissions;

public class AbpOpenIddictProPermissions
{
    public const string GroupName = "OpenIddictPro";

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(AbpOpenIddictProPermissions));
    }

    public static class Application
    {
        public const string Default = "OpenIddictPro.Application";
        public const string Delete = "OpenIddictPro.Application.Delete";
        public const string Update = "OpenIddictPro.Application.Update";
        public const string Create = "OpenIddictPro.Application.Create";
        public const string ManagePermissions = "OpenIddictPro.Application.ManagePermissions";
        public const string ViewChangeHistory = "AuditLogging.ViewChangeHistory:Volo.Abp.OpenIddict.Pro.Applications.Application";
    }

    public static class Scope
    {
        public const string Default = "OpenIddictPro.Scope";
        public const string Delete = "OpenIddictPro.Scope.Delete";
        public const string Update = "OpenIddictPro.Scope.Update";
        public const string Create = "OpenIddictPro.Scope.Create";
        public const string ViewChangeHistory = "AuditLogging.ViewChangeHistory:Volo.Abp.OpenIddict.Pro.Scopes.Scope";
    }
}
