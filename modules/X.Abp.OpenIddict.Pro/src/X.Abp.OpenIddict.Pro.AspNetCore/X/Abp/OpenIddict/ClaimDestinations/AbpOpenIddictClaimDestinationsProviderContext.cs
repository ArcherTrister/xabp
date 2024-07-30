// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Security.Claims;

namespace X.Abp.OpenIddict.ClaimDestinations;

public class AbpOpenIddictClaimDestinationsProviderContext
{
    public IServiceProvider ScopeServiceProvider { get; }

    public ClaimsPrincipal Principal { get; }

    public Claim[] Claims { get; }

    public AbpOpenIddictClaimDestinationsProviderContext(IServiceProvider scopeServiceProvider, ClaimsPrincipal principal, Claim[] claims)
    {
        ScopeServiceProvider = scopeServiceProvider;
        Principal = principal;
        Claims = claims;
    }
}
