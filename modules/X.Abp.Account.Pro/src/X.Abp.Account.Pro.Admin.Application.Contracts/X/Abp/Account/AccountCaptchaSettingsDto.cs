// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.ComponentModel.DataAnnotations;

namespace X.Abp.Account;

public class AccountCaptchaSettingsDto
{
    public bool UseCaptchaOnLogin { get; set; }

    public bool UseCaptchaOnRegistration { get; set; }

    public string VerifyBaseUrl { get; set; }

    public string SiteKey { get; set; }

    public string SiteSecret { get; set; }

    [Range(2, 3)]
    public int Version { get; set; }

    [Range(0.1, 1.0)]
    public double Score { get; set; }
}
