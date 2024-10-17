// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.GlobalFeatures;

namespace X.Abp.CmsKit.GlobalFeatures;

public class GlobalCmsKitProFeatures : GlobalModuleFeatures
{
    public const string ModuleName = "CmsKitPro";

    public NewslettersFeature Newsletter => GetFeature<NewslettersFeature>();

    public ContactFeature Contact => GetFeature<ContactFeature>();

    public UrlShortingFeature UrlShortingFeature => GetFeature<UrlShortingFeature>();

    public PollsFeature PollsFeature => GetFeature<PollsFeature>();

    public PageFeedbackFeature PageFeedbackFeature => GetFeature<PageFeedbackFeature>();

    public FaqFeature FaqFeature => GetFeature<FaqFeature>();

    public GlobalCmsKitProFeatures(GlobalFeatureManager cmsKitPro)
      : base(cmsKitPro)
    {
        AddFeature(new NewslettersFeature(this));
        AddFeature(new ContactFeature(this));
        AddFeature(new UrlShortingFeature(this));
        AddFeature(new PollsFeature(this));
        AddFeature(new PageFeedbackFeature(this));
        AddFeature(new FaqFeature(this));
    }
}
