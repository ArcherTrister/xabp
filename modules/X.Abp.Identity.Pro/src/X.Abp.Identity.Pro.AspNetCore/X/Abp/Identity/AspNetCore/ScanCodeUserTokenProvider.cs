// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace X.Abp.Identity.AspNetCore;

public class ScanCodeUserTokenProvider : DataProtectorTokenProvider<IdentityUser>
{
    public ScanCodeUserTokenProvider(
        IDataProtectionProvider dataProtectionProvider,
        IOptions<DataProtectionTokenProviderOptions> options,
        ILogger<DataProtectorTokenProvider<IdentityUser>> logger)
        : base(dataProtectionProvider, options, logger)
    {
    }
}
