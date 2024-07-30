// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.CmsKit.Pro.Public.Web.Pages.Public.Newsletters;

public class NewsletterPreferenceDetailsViewModel
{
    public string Preference { get; set; }

    public string DisplayPreference { get; set; }

    public string DisplayDefinition { get; set; }

    public bool IsSelectedByEmailAddress { get; set; }
}
