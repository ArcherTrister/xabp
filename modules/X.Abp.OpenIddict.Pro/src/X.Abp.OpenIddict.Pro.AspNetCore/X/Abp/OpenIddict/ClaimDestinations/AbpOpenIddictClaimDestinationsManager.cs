// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Volo.Abp.DependencyInjection;

namespace X.Abp.OpenIddict.ClaimDestinations;

public class AbpOpenIddictClaimDestinationsManager : ISingletonDependency
{
    protected IServiceScopeFactory ServiceScopeFactory { get; }

    protected IOptions<AbpOpenIddictClaimDestinationsOptions> Options { get; }

    public AbpOpenIddictClaimDestinationsManager(IServiceScopeFactory serviceScopeFactory, IOptions<AbpOpenIddictClaimDestinationsOptions> options)
    {
        ServiceScopeFactory = serviceScopeFactory;
        Options = options;
    }

    public virtual async Task SetAsync(ClaimsPrincipal principal)
    {
        using var scope = ServiceScopeFactory.CreateScope();
        foreach (var providerType in Options.Value.ClaimDestinationsProvider)
        {
            var provider = (IAbpOpenIddictClaimDestinationsProvider)scope.ServiceProvider.GetRequiredService(providerType);
            await provider.SetDestinationsAsync(new AbpOpenIddictClaimDestinationsProviderContext(scope.ServiceProvider, principal, principal.Claims.ToArray()));
        }
    }
}
