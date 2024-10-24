// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Security.Claims;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Security.Claims;

namespace X.Abp.OpenIddict.Security;

[Dependency(ReplaceServices = true)]
public class FakeCurrentPrincipalAccessor : ThreadCurrentPrincipalAccessor
{
    private static readonly object _lockObject = new object();

    protected override ClaimsPrincipal GetClaimsPrincipal()
    {
        return GetPrincipal();
    }

    private ClaimsPrincipal _principal;

    private ClaimsPrincipal GetPrincipal()
    {
        if (_principal == null)
        {
            lock (_lockObject)
            {
                if (_principal == null)
                {
                    _principal = new ClaimsPrincipal(
                        new ClaimsIdentity(
                            new List<Claim>
                            {
                                    new Claim(AbpClaimTypes.UserId, "2e701e62-0953-4dd3-910b-dc6cc93ccb0d"),
                                    new Claim(AbpClaimTypes.UserName, "admin"),
                                    new Claim(AbpClaimTypes.Email, "admin@abp.io")
                            }));
                }
            }
        }

        return _principal;
    }
}
