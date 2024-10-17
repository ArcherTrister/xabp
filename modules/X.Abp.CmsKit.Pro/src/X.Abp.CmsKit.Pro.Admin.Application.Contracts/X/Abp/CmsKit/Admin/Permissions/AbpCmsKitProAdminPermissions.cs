// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Reflection;

namespace X.Abp.CmsKit.Admin.Permissions;

public static class AbpCmsKitProAdminPermissions
{
    public const string GroupName = "CmsKit";

    public static class Newsletters
    {
        public const string Default = GroupName + ".Newsletter";
        public const string EditPreferences = Default + ".EditPreferences";
        public const string Import = Default + ".Import";
    }

    public static class Contact
    {
        private const string Default = "CmsKit.Contact";
        public const string SettingManagement = "CmsKit.SettingManagement";
    }

    public static class UrlShorting
    {
        public const string Default = GroupName + ".UrlShorting";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static class Polls
    {
        public const string Default = GroupName + ".Poll";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static class PageFeedbacks
    {
        public const string Default = GroupName + ".PageFeedback";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
        public const string Settings = Default + ".Settings";
    }

    public static class Faqs
    {
        public const string Default = GroupName + ".Faq";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(AbpCmsKitProAdminPermissions));
    }
}
