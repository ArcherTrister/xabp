// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Security.Claims;

using Volo.Abp.DependencyInjection;
using Volo.Abp.Security.Claims;

namespace AbpVnext.Pro.Security;

[Dependency(ReplaceServices = true)]
public class FakeCurrentPrincipalAccessor : ICurrentPrincipalAccessor, ISingletonDependency
{
    public ClaimsPrincipal Principal => GetPrincipal();

    private ClaimsPrincipal _principal;

    private ClaimsPrincipal GetPrincipal()
    {
        if (_principal == null)
        {
            var fakeCurrentPrincipalAccessor = this;
            lock (fakeCurrentPrincipalAccessor)
            {
                _principal ??= new ClaimsPrincipal(
                        new ClaimsIdentity(
                            new List<Claim>
                            {
                                    new Claim(AbpClaimTypes.UserId, "2e701e62-0953-4dd3-910b-dc6cc93ccb0d"),
                                    new Claim(AbpClaimTypes.UserName, "admin"),
                                    new Claim(AbpClaimTypes.Email, "admin@abp.io")
                            }));
            }
        }

        return _principal;
    }

    public IDisposable Change(ClaimsPrincipal principal)
    {
        _principal = principal;
        return null;
    }
}
