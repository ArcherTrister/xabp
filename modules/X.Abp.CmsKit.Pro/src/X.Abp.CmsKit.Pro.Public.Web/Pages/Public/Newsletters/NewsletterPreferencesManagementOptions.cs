// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Localization;

namespace X.Abp.CmsKit.Pro.Public.Web.Pages.Public.Newsletters;

public class NewsletterPreferencesManagementOptions
{
    public ILocalizableString PrivacyPolicyConfirmation { get; set; }

    public string Source { get; set; }
}
