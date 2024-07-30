// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;

using Volo.Abp;
using Volo.Abp.Localization;

namespace X.Abp.CmsKit.Newsletters;

public class NewsletterPreferenceDefinition
{
    public string Preference { get; set; }

    public ILocalizableString DisplayPreference { get; }

    public ILocalizableString PrivacyPolicyConfirmation { get; set; }

    public ILocalizableString Definition { get; set; }

    public IReadOnlyList<string> AdditionalPreferences { get; set; }

    public string WidgetViewPath { get; set; }

    public NewsletterPreferenceDefinition(
      ILocalizableString displayPreference,
      ILocalizableString definition = null,
      ILocalizableString privacyPolicyConfirmation = null,
      IReadOnlyList<string> additionalPreferences = null,
      string widgetViewPath = null)
    {
        DisplayPreference = Check.NotNull(displayPreference, nameof(displayPreference));
        Definition = definition;
        PrivacyPolicyConfirmation = privacyPolicyConfirmation;
        AdditionalPreferences = additionalPreferences;
        WidgetViewPath = widgetViewPath;
    }
}
