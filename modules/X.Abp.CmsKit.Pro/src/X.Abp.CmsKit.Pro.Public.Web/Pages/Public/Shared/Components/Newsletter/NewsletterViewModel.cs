// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;

namespace X.Abp.CmsKit.Pro.Public.Web.Pages.Public.Shared.Components.Newsletter;

public class NewsletterViewModel
{
    public string Preference { get; set; }

    public string Source { get; set; }

    public string NormalizedSource { get; set; }

    public string PrivacyPolicyConfirmation { get; set; }

    public bool RequestAdditionalPreferencesLater { get; set; }

    public List<string> AdditionalPreferences { get; set; }

    public List<string> DisplayAdditionalPreferences { get; set; }
}
