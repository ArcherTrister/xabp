// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Security.Claims;

namespace X.Abp.Identity.ExternalLoginProviders.OAuth;

public class AbpOAuthExternalLoginProviderOptions
{
    public string NameClaimType { get; set; } = AbpClaimTypes.Name;

    public string SurnameClaimType { get; set; } = AbpClaimTypes.SurName;

    public string EmailClaimType { get; set; } = AbpClaimTypes.Email;

    public string EmailConfirmedClaimType { get; set; } = AbpClaimTypes.EmailVerified;

    public string PhoneNumberClaimType { get; set; } = AbpClaimTypes.PhoneNumber;

    public string PhoneNumberConfirmedClaimType { get; set; } = AbpClaimTypes.PhoneNumberVerified;

    public string ProviderKeyClaimType { get; set; } = AbpClaimTypes.UserId;

    public bool CanObtainUserInfoWithoutPassword { get; set; }
}
