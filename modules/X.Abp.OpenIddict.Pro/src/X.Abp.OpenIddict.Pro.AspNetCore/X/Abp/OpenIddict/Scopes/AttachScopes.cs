// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Linq;
using System.Threading.Tasks;

using OpenIddict.Server;

using Volo.Abp.OpenIddict.Scopes;

namespace X.Abp.OpenIddict.Scopes;

public class AttachScopes : IOpenIddictServerHandler<OpenIddictServerEvents.HandleConfigurationRequestContext>
{
    public static OpenIddictServerHandlerDescriptor Descriptor { get; }
        = OpenIddictServerHandlerDescriptor.CreateBuilder<OpenIddictServerEvents.HandleConfigurationRequestContext>()
            .UseSingletonHandler<AttachScopes>()
            .SetOrder(OpenIddictServerHandlers.Discovery.AttachScopes.Descriptor.Order + 1)
            .SetType(OpenIddictServerHandlerType.Custom)
            .Build();

    private readonly IOpenIddictScopeRepository _scopeRepository;

    public AttachScopes(IOpenIddictScopeRepository scopeRepository)
    {
        _scopeRepository = scopeRepository;
    }

    public async ValueTask HandleAsync(OpenIddictServerEvents.HandleConfigurationRequestContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var scopes = await _scopeRepository.GetListAsync();
        context.Scopes.UnionWith(scopes.Select(x => x.Name));
    }
}
