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
            featureGroupDefinition.AddFeature("CmsKitPro.ContactEnable", "true", L("Feature:ContactEnable"), L("Feature:ContactEnableDescription"), new ToggleStringValueType());
        }

        if (GlobalFeatureManager.Instance.IsEnabled<NewslettersFeature>())
        {
            featureGroupDefinition.AddFeature("CmsKitPro.NewsletterEnable", "true", L("Feature:NewsletterEnable"), L("Feature:NewsletterEnableDescription"), new ToggleStringValueType());
        }

        if (GlobalFeatureManager.Instance.IsEnabled<PollsFeature>())
        {
            featureGroupDefinition.AddFeature("CmsKitPro.PollEnable", "true", L("Feature:PollEnable"), L("Feature:PollEnableDescription"), new ToggleStringValueType());
        }

        if (GlobalFeatureManager.Instance.IsEnabled<UrlShortingFeature>())
        {
            featureGroupDefinition.AddFeature("CmsKitPro.UrlShortingEnable", "true", L("Feature:UrlShortingEnable"), L("Feature:UrlShortingEnableDescription"), new ToggleStringValueType());
        }

        if (GlobalFeatureManager.Instance.IsEnabled<PageFeedbackFeature>())
        {
            featureGroupDefinition.AddFeature("CmsKitPro.PageFeedbackEnable", "true", L("Feature:PageFeedbackEnable"), L("Feature:PageFeedbackEnableDescription"), new ToggleStringValueType());
        }
    }

    private static LocalizableString L(string name) => LocalizableString.Create<CmsKitResource>(name);
}
