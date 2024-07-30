// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp.Authorization.Permissions;
using Volo.Abp.GlobalFeatures;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;
using Volo.CmsKit.Localization;

using X.Abp.CmsKit.GlobalFeatures;

namespace X.Abp.CmsKit.Admin.Permissions;

public class AbpCmsKitProAdminPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var cmsKitPermission = context.GetGroupOrNull(AbpCmsKitProAdminPermissions.GroupName) ?? context.AddGroup(AbpCmsKitProAdminPermissions.GroupName, L("Permission:CmsKit"));

        // Newsletter
        cmsKitPermission
            .AddPermission(AbpCmsKitProAdminPermissions.Newsletters.Default, L("Permission:NewsletterManagement"), MultiTenancySides.Both, true)
            .RequireGlobalFeatures(new Type[]
            {
                typeof(NewslettersFeature)
            });

        // Poll
        var pollPermission = cmsKitPermission
            .AddPermission(AbpCmsKitProAdminPermissions.Polls.Default, L("Permission:Poll"), MultiTenancySides.Both, true)
            .RequireGlobalFeatures(new Type[]
            {
                typeof(PollsFeature)
            });
        pollPermission.AddChild(AbpCmsKitProAdminPermissions.Polls.Create, L("Permission:Poll.Create"), MultiTenancySides.Both, true)
            .RequireGlobalFeatures(new Type[]
            {
                typeof(PollsFeature)
            });
        pollPermission.AddChild(AbpCmsKitProAdminPermissions.Polls.Update, L("Permission:Poll.Update"), MultiTenancySides.Both, true)
            .RequireGlobalFeatures(new Type[]
            {
                typeof(PollsFeature)
            });
        pollPermission.AddChild(AbpCmsKitProAdminPermissions.Polls.Delete, L("Permission:Poll.Delete"), MultiTenancySides.Both, true)
            .RequireGlobalFeatures(new Type[]
            {
                typeof(PollsFeature)
            });

        // Contact
        cmsKitPermission.AddPermission(AbpCmsKitProAdminPermissions.Contact.SettingManagement, L("Permission:Contact:SettingManagement"), MultiTenancySides.Both, true)
            .RequireGlobalFeatures(new Type[]
            {
                typeof(ContactFeature)
            });

        // UrlShorting
        var urlShortingPermission = cmsKitPermission.AddPermission(AbpCmsKitProAdminPermissions.UrlShorting.Default, L("Permission:UrlShorting"), MultiTenancySides.Both, true)
            .RequireGlobalFeatures(new Type[]
            {
                typeof(UrlShortingFeature)
            });
        urlShortingPermission.AddChild(AbpCmsKitProAdminPermissions.UrlShorting.Create, L("Permission:UrlShorting.Create"), MultiTenancySides.Both, true)
            .RequireGlobalFeatures(new Type[]
            {
                typeof(UrlShortingFeature)
            });
        urlShortingPermission.AddChild(AbpCmsKitProAdminPermissions.UrlShorting.Update, L("Permission:UrlShorting.Update"), MultiTenancySides.Both, true)
            .RequireGlobalFeatures(new Type[]
            {
                typeof(UrlShortingFeature)
            });
        urlShortingPermission.AddChild(AbpCmsKitProAdminPermissions.UrlShorting.Delete, L("Permission:UrlShorting.Delete"), MultiTenancySides.Both, true)
            .RequireGlobalFeatures(new Type[]
            {
                typeof(UrlShortingFeature)
            });
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<CmsKitResource>(name);
    }
}
