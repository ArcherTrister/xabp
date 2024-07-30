// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.Extensions.Options;

using OpenIddict.Abstractions;
using OpenIddict.Server;

using Volo.Abp;

namespace X.Abp.OpenIddict.WildcardDomains;

public class AbpValidateAuthorizedParty : AbpOpenIddictWildcardDomainBase<OpenIddictServerHandlers.Session.ValidateAuthorizedParty, OpenIddictServerEvents.ValidateLogoutRequestContext>
{
    public static OpenIddictServerHandlerDescriptor Descriptor { get; }
        = OpenIddictServerHandlerDescriptor.CreateBuilder<OpenIddictServerEvents.ValidateLogoutRequestContext>()
            .UseScopedHandler<AbpValidateAuthorizedParty>()
            .SetOrder(OpenIddictServerHandlers.Session.ValidateToken.Descriptor.Order + 1_000)
            .SetType(OpenIddictServerHandlerType.BuiltIn)
            .Build();

    public AbpValidateAuthorizedParty(
        IOptions<AbpOpenIddictWildcardDomainOptions> wildcardDomainsOptions,
        IOpenIddictApplicationManager applicationManager)
        : base(wildcardDomainsOptions, new OpenIddictServerHandlers.Session.ValidateAuthorizedParty(applicationManager))
    {
        Handler = new OpenIddictServerHandlers.Session.ValidateAuthorizedParty(applicationManager);
    }

    public override async ValueTask HandleAsync(OpenIddictServerEvents.ValidateLogoutRequestContext context)
    {
        Check.NotNull(context, nameof(context));

        if (await CheckWildcardDomainAsync(context.PostLogoutRedirectUri))
        {
            return;
        }

        await Handler.HandleAsync(context);
    }
}
