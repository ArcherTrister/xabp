// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.Options;

using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Widgets;

namespace X.Abp.Gdpr.Web.Pages.Gdpr.Components.CookieConsent;

[Widget(ScriptFiles = new string[] { "/Pages/Gdpr/Components/CookieConsent/Default.js" })]
public class AbpCookieConsentViewComponent : AbpViewComponent
{
    protected AbpCookieConsentOptions CookieConsentOptions { get; }

    public AbpCookieConsentViewComponent(
      IOptions<AbpCookieConsentOptions> cookieConsentOptions)
    {
        CookieConsentOptions = cookieConsentOptions.Value;
    }

    public IViewComponentResult Invoke()
    {
        var trackingConsentFeature = HttpContext.Features.Get<ITrackingConsentFeature>();
        return !CookieConsentOptions.IsEnabled || trackingConsentFeature == null || trackingConsentFeature.CanTrack
            ? new ContentViewComponentResult(string.Empty)
            : View("~/Pages/Gdpr/Components/CookieConsent/Default.cshtml", new AbpCookieConsentViewModel
            {
                Options = CookieConsentOptions,
                CookieString = trackingConsentFeature.CreateConsentCookie()
            });
    }
}
