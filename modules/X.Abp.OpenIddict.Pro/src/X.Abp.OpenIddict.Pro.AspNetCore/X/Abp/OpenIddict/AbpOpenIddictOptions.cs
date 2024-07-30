// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Security.Claims;

namespace X.Abp.OpenIddict;

public class AbpOpenIddictAspNetCoreOptions
{
    /// <summary>
    /// Updates <see cref="AbpClaimTypes"/> to be compatible with OpenIddict claims.
    /// Default: true.
    /// </summary>
    public bool UpdateAbpClaimTypes { get; set; } = true;

    /// <summary>
    /// Set false to suppress AddDeveloperSigningCredential() call on the OpenIddictBuilder.
    /// Default: true.
    /// </summary>
    public bool AddDevelopmentEncryptionAndSigningCertificate { get; set; } = true;
}
