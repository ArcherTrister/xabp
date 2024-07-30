// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using OpenIddict.Abstractions;
using OpenIddict.Server;

namespace X.Abp.OpenIddict;

public class RemoveClaimsFromClientCredentialsGrantType : IOpenIddictServerHandler<OpenIddictServerEvents.ProcessSignInContext>
{
    public static OpenIddictServerHandlerDescriptor Descriptor { get; }
        = OpenIddictServerHandlerDescriptor.CreateBuilder<OpenIddictServerEvents.ProcessSignInContext>()
            .AddFilter<OpenIddictServerHandlerFilters.RequireAccessTokenGenerated>()
            .UseSingletonHandler<RemoveClaimsFromClientCredentialsGrantType>()
            .SetOrder(OpenIddictServerHandlers.PrepareAccessTokenPrincipal.Descriptor.Order - 1)
            .SetType(OpenIddictServerHandlerType.Custom)
            .Build();

    public ValueTask HandleAsync(OpenIddictServerEvents.ProcessSignInContext context)
    {
        if (context.Request.IsClientCredentialsGrantType())
        {
            if (context.Principal != null)
            {
                context.Principal.RemoveClaims(OpenIddictConstants.Claims.Subject);
                context.Principal.RemoveClaims(OpenIddictConstants.Claims.PreferredUsername);
            }
        }

        return default;
    }
}
