// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Features;
using Volo.Abp.GlobalFeatures;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;
using Volo.CmsKit.Localization;

using X.Abp.CmsKit.Features;
using X.Abp.CmsKit.GlobalFeatures;

namespace X.Abp.CmsKit.Admin.Permissions;

public class AbpCmsKitProAdminPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var cmsKitPermission = context.GetGroupOrNull(AbpCmsKitProAdminPermissions.GroupName) ?? context.AddGroup(AbpCmsKitProAdminPermissions.GroupName, L("Permission:CmsKit"));

        // Newsletter
        PermissionDefinition newsletterPermissionDefinition = cmsKitPermission.AddPermission(AbpCmsKitProAdminPermissions.Newsletters.Default, L("Permission:NewsletterManagement"), MultiTenancySides.Both, true)
            .RequireGlobalFeatures(typeof(NewslettersFeature))
            .RequireFeatures(CmsKitProFeatures.NewsletterEnable);

        newsletterPermissionDefinition.AddChild(AbpCmsKitProAdminPermissions.Newsletters.EditPreferences, L("Permission:Newsletter.EditPreferences"))
            .RequireGlobalFeatures(typeof(NewslettersFeature))
            .RequireFeatures(CmsKitProFeatures.NewsletterEnable);
        newsletterPermissionDefinition.AddChild(AbpCmsKitProAdminPermissions.Newsletters.Import, L("Permission:Newsletter.Import"))
            .RequireGlobalFeatures(typeof(NewslettersFeature))
            .RequireFeatures(CmsKitProFeatures.NewsletterEnable);

        // Poll
        var pollPermission = cmsKitPermission.AddPermission(AbpCmsKitProAdminPermissions.Polls.Default, L("Permission:Poll"), MultiTenancySides.Both, true)
            .RequireGlobalFeatures(typeof(PollsFeature))
            .RequireFeatures(CmsKitProFeatures.PollEnable);
        pollPermission.AddChild(AbpCmsKitProAdminPermissions.Polls.Create, L("Permission:Poll.Create"), MultiTenancySides.Both, true)
            .RequireGlobalFeatures(typeof(PollsFeature))
            .RequireFeatures(CmsKitProFeatures.PollEnable);
        pollPermission.AddChild(AbpCmsKitProAdminPermissions.Polls.Update, L("Permission:Poll.Update"), MultiTenancySides.Both, true)
            .RequireGlobalFeatures(typeof(PollsFeature))
            .RequireFeatures(CmsKitProFeatures.PollEnable);
        pollPermission.AddChild(AbpCmsKitProAdminPermissions.Polls.Delete, L("Permission:Poll.Delete"), MultiTenancySides.Both, true)
            .RequireGlobalFeatures(typeof(PollsFeature))
            .RequireFeatures(CmsKitProFeatures.PollEnable);

        // Contact
        cmsKitPermission.AddPermission(AbpCmsKitProAdminPermissions.Contact.SettingManagement, L("Permission:Contact:SettingManagement"), MultiTenancySides.Both, true)
            .RequireGlobalFeatures(typeof(ContactFeature))
            .RequireFeatures(CmsKitProFeatures.ContactEnable);

        // UrlShorting
        var urlShortingPermission = cmsKitPermission.AddPermission(AbpCmsKitProAdminPermissions.UrlShorting.Default, L("Permission:UrlShorting"), MultiTenancySides.Both, true)
            .RequireGlobalFeatures(typeof(UrlShortingFeature))
            .RequireFeatures(CmsKitProFeatures.UrlShortingEnable);
        urlShortingPermission.AddChild(AbpCmsKitProAdminPermissions.UrlShorting.Create, L("Permission:UrlShorting.Create"), MultiTenancySides.Both, true)
            .RequireGlobalFeatures(typeof(UrlShortingFeature))
            .RequireFeatures(CmsKitProFeatures.UrlShortingEnable);
        urlShortingPermission.AddChild(AbpCmsKitProAdminPermissions.UrlShorting.Update, L("Permission:UrlShorting.Update"), MultiTenancySides.Both, true)
            .RequireGlobalFeatures(typeof(UrlShortingFeature))
            .RequireFeatures(CmsKitProFeatures.UrlShortingEnable);
        urlShortingPermission.AddChild(AbpCmsKitProAdminPermissions.UrlShorting.Delete, L("Permission:UrlShorting.Delete"), MultiTenancySides.Both, true)
            .RequireGlobalFeatures(typeof(UrlShortingFeature))
            .RequireFeatures(CmsKitProFeatures.UrlShortingEnable);

        // PageFeedback
        var pageFeedbackPermission = cmsKitPermission.AddPermission(AbpCmsKitProAdminPermissions.PageFeedbacks.Default, L("Permission:PageFeedback"), MultiTenancySides.Both, true)
            .RequireGlobalFeatures(typeof(PageFeedbackFeature))
            .RequireFeatures(CmsKitProFeatures.PageFeedbackEnable);
        pageFeedbackPermission.AddChild(AbpCmsKitProAdminPermissions.PageFeedbacks.Update, L("Permission:PageFeedback.Update"), MultiTenancySides.Both, true)
            .RequireGlobalFeatures(typeof(PageFeedbackFeature))
            .RequireFeatures(CmsKitProFeatures.PageFeedbackEnable);
        pageFeedbackPermission.AddChild(AbpCmsKitProAdminPermissions.PageFeedbacks.Delete, L("Permission:PageFeedback.Delete"), MultiTenancySides.Both, true)
            .RequireGlobalFeatures(typeof(PageFeedbackFeature))
            .RequireFeatures(CmsKitProFeatures.PageFeedbackEnable);
        pageFeedbackPermission.AddChild(AbpCmsKitProAdminPermissions.PageFeedbacks.Settings, L("Permission:PageFeedback.Settings"), MultiTenancySides.Both, true)
            .RequireGlobalFeatures(typeof(PageFeedbackFeature))
            .RequireFeatures(CmsKitProFeatures.PageFeedbackEnable);

        // Faq
        var faqPermission = cmsKitPermission.AddPermission(AbpCmsKitProAdminPermissions.Faqs.Default, L("Permission:Faq"), MultiTenancySides.Both, true)
            .RequireGlobalFeatures(typeof(FaqFeature))
            .RequireFeatures(CmsKitProFeatures.FaqEnable);
        faqPermission.AddChild(AbpCmsKitProAdminPermissions.Faqs.Create, L("Permission:Faq.Create"), MultiTenancySides.Both, true)
            .RequireGlobalFeatures(typeof(FaqFeature))
            .RequireFeatures(CmsKitProFeatures.FaqEnable);
        faqPermission.AddChild(AbpCmsKitProAdminPermissions.Faqs.Update, L("Permission:Faq.Update"), MultiTenancySides.Both, true)
            .RequireGlobalFeatures(typeof(FaqFeature))
            .RequireFeatures(CmsKitProFeatures.FaqEnable);
        faqPermission.AddChild(AbpCmsKitProAdminPermissions.Faqs.Delete, L("Permission:Faq.Delete"), MultiTenancySides.Both, true)
            .RequireGlobalFeatures(typeof(FaqFeature))
            .RequireFeatures(CmsKitProFeatures.FaqEnable);
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<CmsKitResource>(name);
    }
}
