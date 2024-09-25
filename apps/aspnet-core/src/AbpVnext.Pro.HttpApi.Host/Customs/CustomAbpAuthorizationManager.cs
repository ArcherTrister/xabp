using System.Collections.Immutable;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using OpenIddict.Abstractions;
using OpenIddict.Core;

using Volo.Abp.DependencyInjection;
using Volo.Abp.OpenIddict;
using Volo.Abp.OpenIddict.Authorizations;

namespace AbpVnext.Pro.Customs;

//ReplaceAuthorizationManager
[Dependency(ReplaceServices = true)]
[ExposeServices(typeof(AbpAuthorizationManager))]
public class CustomAbpAuthorizationManager : AbpAuthorizationManager
{
    public CustomAbpAuthorizationManager(IOpenIddictAuthorizationCache<OpenIddictAuthorizationModel> cache, ILogger<OpenIddictAuthorizationManager<OpenIddictAuthorizationModel>> logger, IOptionsMonitor<OpenIddictCoreOptions> options, IOpenIddictAuthorizationStoreResolver resolver, AbpOpenIddictIdentifierConverter identifierConverter) : base(cache, logger, options, resolver, identifierConverter)
    {
    }

    public override ValueTask<long> CountAsync(CancellationToken cancellationToken = default)
    {
        return base.CountAsync(cancellationToken);
    }

    public override ValueTask<OpenIddictAuthorizationModel> CreateAsync(OpenIddictAuthorizationDescriptor descriptor, CancellationToken cancellationToken = default)
    {
        return base.CreateAsync(descriptor, cancellationToken);
    }

    public override ValueTask CreateAsync(OpenIddictAuthorizationModel authorization, CancellationToken cancellationToken = default)
    {
        return base.CreateAsync(authorization, cancellationToken);
    }

    public override ValueTask<OpenIddictAuthorizationModel> CreateAsync(ClaimsIdentity identity, string subject, string client, string type, ImmutableArray<string> scopes, CancellationToken cancellationToken = default)
    {
        return base.CreateAsync(identity, subject, client, type, scopes, cancellationToken);
    }

    public override ValueTask<OpenIddictAuthorizationModel> CreateAsync(ClaimsPrincipal principal, string subject, string client, string type, ImmutableArray<string> scopes, CancellationToken cancellationToken = default)
    {
        return base.CreateAsync(principal, subject, client, type, scopes, cancellationToken);
    }

    public async override ValueTask UpdateAsync(OpenIddictAuthorizationModel authorization, CancellationToken cancellationToken = default)
    {
        if (!Options.CurrentValue.DisableEntityCaching)
        {
            var entity = await Store.FindByIdAsync(IdentifierConverter.ToString(authorization.Id), cancellationToken);
            if (entity != null)
            {
                await Cache.RemoveAsync(entity, cancellationToken);
            }
        }

        await base.UpdateAsync(authorization, cancellationToken);
    }
}
