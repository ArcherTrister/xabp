// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Widgets;
using Volo.Abp.Localization;

using X.Abp.CmsKit.Public.Newsletters;

namespace X.Abp.CmsKit.Pro.Public.Web.Pages.Public.Shared.Components.Newsletter;

[Widget(AutoInitialize = true, ScriptTypes = new Type[] { typeof(NewsletterWidgetScriptContributor) }, StyleFiles = new string[] { "/Pages/Public/Shared/Components/Newsletter/Default.css" })]
[ViewComponent(Name = "CmsNewsletter")]
public class NewsletterViewComponent : AbpViewComponent
{
    protected INewsletterRecordPublicAppService NewsletterRecordPublicAppService { get; }

    protected IStringLocalizerFactory StringLocalizerFactory { get; }

    public NewsletterViewComponent(
      INewsletterRecordPublicAppService newsletterRecordPublicAppService,
      IStringLocalizerFactory stringLocalizerFactory)
    {
        NewsletterRecordPublicAppService = newsletterRecordPublicAppService;
        StringLocalizerFactory = stringLocalizerFactory;
    }

    public virtual async Task<IViewComponentResult> InvokeAsync(
      string preference,
      string source,
      bool requestAdditionalPreferencesLater,
      ILocalizableString privacyPolicyConfirmation)
    {
        Check.NotNullOrWhiteSpace(preference, nameof(preference));
        Check.NotNullOrWhiteSpace(source, nameof(source));

        var newsletterEmailOptions = await NewsletterRecordPublicAppService.GetOptionByPreferenceAsync(preference);

        var model = new NewsletterViewModel()
        {
            Preference = preference,
            Source = source,
            NormalizedSource = source.Replace('.', '_'),
            PrivacyPolicyConfirmation = privacyPolicyConfirmation == null ? newsletterEmailOptions.PrivacyPolicyConfirmation : privacyPolicyConfirmation.Localize(StringLocalizerFactory).Value,
            RequestAdditionalPreferencesLater = requestAdditionalPreferencesLater,
            AdditionalPreferences = newsletterEmailOptions.AdditionalPreferences,
            DisplayAdditionalPreferences = newsletterEmailOptions.DisplayAdditionalPreferences
        };

        newsletterEmailOptions.WidgetViewPath ??= "~/Pages/Public/Shared/Components/Newsletter/Default.cshtml";

        return View(newsletterEmailOptions.WidgetViewPath, model);
    }
}
