// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.Extensions.Options;

using OpenIddict.Server;

using Volo.Abp;

namespace X.Abp.OpenIddict.WildcardDomains;

public class AbpValidateRedirectUriParameter : AbpOpenIddictWildcardDomainBase<OpenIddictServerHandlers.Authentication.ValidateRedirectUriParameter, OpenIddictServerEvents.ValidateAuthorizationRequestContext>
{
    public static OpenIddictServerHandlerDescriptor Descriptor { get; }
        = OpenIddictServerHandlerDescriptor.CreateBuilder<OpenIddictServerEvents.ValidateAuthorizationRequestContext>()
            .UseSingletonHandler<AbpValidateRedirectUriParameter>()
            .SetOrder(OpenIddictServerHandlers.Authentication.ValidateClientIdParameter.Descriptor.Order + 1_000)
            .SetType(OpenIddictServerHandlerType.BuiltIn)
            .Build();

    public AbpValidateRedirectUriParameter(IOptions<AbpOpenIddictWildcardDomainOptions> wildcardDomainsOptions)
        : base(wildcardDomainsOptions, new OpenIddictServerHandlers.Authentication.ValidateRedirectUriParameter())
    {
    }

    public override async ValueTask HandleAsync(OpenIddictServerEvents.ValidateAuthorizationRequestContext context)
    {
        Check.NotNull(context, nameof(context));

        if (!string.IsNullOrEmpty(context.RedirectUri) && await CheckWildcardDomainAsync(context.RedirectUri))
        {
            return;
        }

        await Handler.HandleAsync(context);
    }
}
