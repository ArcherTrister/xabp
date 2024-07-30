// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.Gdpr;

public class AbpCookieConsentOptions
{
    public bool IsEnabled { get; set; }

    public string CookiePolicyUrl { get; set; }

    public string PrivacyPolicyUrl { get; set; }
}
