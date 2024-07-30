// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RequestLocalization;
using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;

namespace X.Abp.Quartz.Web.Components.Toolbar.LanguageSwitch;

public class LanguageSwitchViewComponent : AbpViewComponent
{
    protected ILanguageProvider LanguageProvider { get; }

    public LanguageSwitchViewComponent(ILanguageProvider languageProvider)
    {
        LanguageProvider = languageProvider;
    }

    public virtual async Task<IViewComponentResult> InvokeAsync()
    {
        var languages = await LanguageProvider.GetLanguagesAsync();
        var currentLanguage = languages.FindByCulture(
            CultureInfo.CurrentCulture.Name,
            CultureInfo.CurrentUICulture.Name);

        if (currentLanguage == null)
        {
            var abpRequestLocalizationOptionsProvider = HttpContext.RequestServices.GetRequiredService<IAbpRequestLocalizationOptionsProvider>();
            var localizationOptions = await abpRequestLocalizationOptionsProvider.GetLocalizationOptionsAsync();
            currentLanguage = localizationOptions.DefaultRequestCulture != null
                ? new LanguageInfo(
                    localizationOptions.DefaultRequestCulture.Culture.Name,
                    localizationOptions.DefaultRequestCulture.UICulture.Name,
                    localizationOptions.DefaultRequestCulture.UICulture.DisplayName)
                : new LanguageInfo(
                    CultureInfo.CurrentCulture.Name,
                    CultureInfo.CurrentUICulture.Name,
                    CultureInfo.CurrentUICulture.DisplayName);
        }

        var model = new LanguageSwitchViewComponentModel
        {
            CurrentLanguage = currentLanguage,
            OtherLanguages = languages.Where(l => l != currentLanguage).ToList()
        };

        return View("~/Components/Toolbar/LanguageSwitch/Default.cshtml", model);
    }
}
