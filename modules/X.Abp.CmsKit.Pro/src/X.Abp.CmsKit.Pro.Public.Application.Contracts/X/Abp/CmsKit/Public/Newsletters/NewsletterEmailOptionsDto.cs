// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;

namespace X.Abp.CmsKit.Public.Newsletters;

public class NewsletterEmailOptionsDto
{
    public string PrivacyPolicyConfirmation { get; set; }

    public string WidgetViewPath { get; set; }

    public List<string> AdditionalPreferences { get; set; }

    public List<string> DisplayAdditionalPreferences { get; set; }

    public NewsletterEmailOptionsDto()
    {
        DisplayAdditionalPreferences = new List<string>();
        AdditionalPreferences = new List<string>();
    }
}
