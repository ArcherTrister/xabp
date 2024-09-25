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
        public const string Default = GroupName + ".Application";
        public const string Delete = Default + ".Delete";
        public const string Update = Default + ".Update";
        public const string Create = Default + ".Create";
        public const string ManagePermissions = Default + ".ManagePermissions";
        public const string ViewChangeHistory = "AuditLogging.ViewChangeHistory:Volo.Abp.OpenIddict.Pro.Applications.Application";
    }

    public static class Scope
    {
        public const string Default = GroupName + ".Scope";
        public const string Delete = Default + ".Delete";
        public const string Update = Default + ".Update";
        public const string Create = Default + ".Create";
        public const string ViewChangeHistory = "AuditLogging.ViewChangeHistory:Volo.Abp.OpenIddict.Pro.Scopes.Scope";
    }
}
