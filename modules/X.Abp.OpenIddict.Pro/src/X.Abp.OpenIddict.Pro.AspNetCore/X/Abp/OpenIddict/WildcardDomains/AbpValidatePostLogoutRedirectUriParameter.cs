// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.Extensions.Options;

using OpenIddict.Server;

using Volo.Abp;

namespace X.Abp.OpenIddict.WildcardDomains;

public class AbpValidatePostLogoutRedirectUriParameter : AbpOpenIddictWildcardDomainBase<OpenIddictServerHandlers.Session.ValidatePostLogoutRedirectUriParameter, OpenIddictServerEvents.ValidateLogoutRequestContext>
{
    public static OpenIddictServerHandlerDescriptor Descriptor { get; }
        = OpenIddictServerHandlerDescriptor.CreateBuilder<OpenIddictServerEvents.ValidateLogoutRequestContext>()
            .UseSingletonHandler<AbpValidatePostLogoutRedirectUriParameter>()
            .SetOrder(int.MinValue + 100_000)
            .SetType(OpenIddictServerHandlerType.BuiltIn)
            .Build();

    public AbpValidatePostLogoutRedirectUriParameter(IOptions<AbpOpenIddictWildcardDomainOptions> wildcardDomainsOptions)
       : base(wildcardDomainsOptions, new OpenIddictServerHandlers.Session.ValidatePostLogoutRedirectUriParameter())
    {
    }

    public override async ValueTask HandleAsync(OpenIddictServerEvents.ValidateLogoutRequestContext context)
    {
        Check.NotNull(context, nameof(context));

        if (string.IsNullOrEmpty(context.PostLogoutRedirectUri) || await CheckWildcardDomainAsync(context.PostLogoutRedirectUri))
        {
            return;
        }

        await Handler.HandleAsync(context);
    }
}
