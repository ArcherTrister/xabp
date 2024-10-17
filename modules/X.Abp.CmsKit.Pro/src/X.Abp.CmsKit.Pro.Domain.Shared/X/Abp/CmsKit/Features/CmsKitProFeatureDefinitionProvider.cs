// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Features;
using Volo.Abp.GlobalFeatures;
using Volo.Abp.Localization;
using Volo.Abp.Validation.StringValues;
using Volo.CmsKit.Features;
using Volo.CmsKit.Localization;

using X.Abp.CmsKit.GlobalFeatures;

namespace X.Abp.CmsKit.Features;

public class CmsKitProFeatureDefinitionProvider : CmsKitFeatureDefinitionProvider
{
    public override void Define(IFeatureDefinitionContext context)
    {
        FeatureGroupDefinition featureGroupDefinition = context.AddGroup(CmsKitProFeatures.GroupName, L("Feature:CmsKitProGroup"));
        if (GlobalFeatureManager.Instance.IsEnabled<ContactFeature>())
        {
            featureGroupDefinition.AddFeature(CmsKitProFeatures.ContactEnable, "true", L("Feature:ContactEnable"), L("Feature:ContactEnableDescription"), new ToggleStringValueType());
        }

        if (GlobalFeatureManager.Instance.IsEnabled<NewslettersFeature>())
        {
            featureGroupDefinition.AddFeature(CmsKitProFeatures.NewsletterEnable, "true", L("Feature:NewsletterEnable"), L("Feature:NewsletterEnableDescription"), new ToggleStringValueType());
        }

        if (GlobalFeatureManager.Instance.IsEnabled<PollsFeature>())
        {
            featureGroupDefinition.AddFeature(CmsKitProFeatures.PollEnable, "true", L("Feature:PollEnable"), L("Feature:PollEnableDescription"), new ToggleStringValueType());
        }

        if (GlobalFeatureManager.Instance.IsEnabled<UrlShortingFeature>())
        {
            featureGroupDefinition.AddFeature(CmsKitProFeatures.UrlShortingEnable, "true", L("Feature:UrlShortingEnable"), L("Feature:UrlShortingEnableDescription"), new ToggleStringValueType());
        }

        if (GlobalFeatureManager.Instance.IsEnabled<PageFeedbackFeature>())
        {
            featureGroupDefinition.AddFeature(CmsKitProFeatures.PageFeedbackEnable, "true", L("Feature:PageFeedbackEnable"), L("Feature:PageFeedbackEnableDescription"), new ToggleStringValueType());
        }

        if (GlobalFeatureManager.Instance.IsEnabled<FaqFeature>())
        {
            featureGroupDefinition.AddFeature(CmsKitProFeatures.FaqEnable, "true", L("Feature:FaqEnable"), L("Feature:FaqEnableDescription"), new ToggleStringValueType());
        }
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<CmsKitResource>(name);
    }
}
