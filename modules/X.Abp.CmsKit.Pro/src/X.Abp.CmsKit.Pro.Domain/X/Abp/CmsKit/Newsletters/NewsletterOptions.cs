// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;

using Volo.Abp;

namespace X.Abp.CmsKit.Newsletters;

public class NewsletterOptions
{
    private string widgetViewPath;

    public string WidgetViewPath
    {
        get => widgetViewPath;
        set => widgetViewPath = Check.NotNullOrWhiteSpace(value, nameof(value));
    }

    public Dictionary<string, NewsletterPreferenceDefinition> Preferences { get; }

    public NewsletterOptions()
    {
        Preferences = new Dictionary<string, NewsletterPreferenceDefinition>();
    }

    public virtual NewsletterOptions AddPreference(
      string preference,
      NewsletterPreferenceDefinition definition)
    {
        Check.NotNullOrWhiteSpace(preference, nameof(preference));
        var preferenceDefinition = definition;
        preferenceDefinition.WidgetViewPath ??= widgetViewPath;

        definition.Preference = preference;
        Preferences.Add(preference, definition);
        return this;
    }
}
