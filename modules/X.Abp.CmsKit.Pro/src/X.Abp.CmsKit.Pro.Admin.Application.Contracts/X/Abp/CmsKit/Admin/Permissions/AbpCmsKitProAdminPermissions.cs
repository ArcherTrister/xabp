// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Reflection;

namespace X.Abp.CmsKit.Admin.Permissions;

public static class AbpCmsKitProAdminPermissions
{
    public const string GroupName = "CmsKit";

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(AbpCmsKitProAdminPermissions));
    }

    public static class Newsletters
    {
        public const string Default = "CmsKit.Newsletter";
    }

    public static class Contact
    {
        public const string SettingManagement = "CmsKit.SettingManagement";
    }

    public static class UrlShorting
    {
        public const string Default = "CmsKit.UrlShorting";
        public const string Create = "CmsKit.UrlShorting.Create";
        public const string Update = "CmsKit.UrlShorting.Update";
        public const string Delete = "CmsKit.UrlShorting.Delete";
    }

    public static class Polls
    {
        public const string Default = "CmsKit.Poll";
        public const string Create = "CmsKit.Poll.Create";
        public const string Update = "CmsKit.Poll.Update";
        public const string Delete = "CmsKit.Poll.Delete";
    }
}
