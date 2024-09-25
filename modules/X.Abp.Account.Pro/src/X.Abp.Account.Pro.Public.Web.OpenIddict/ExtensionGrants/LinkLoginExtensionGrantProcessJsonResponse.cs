// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.AspNetCore.Authentication;

using OpenIddict.Abstractions;
using OpenIddict.Server;

using Volo.Abp;

using static OpenIddict.Server.AspNetCore.OpenIddictServerAspNetCoreHandlerFilters;
using static OpenIddict.Server.AspNetCore.OpenIddictServerAspNetCoreHandlers;
using static OpenIddict.Server.OpenIddictServerEvents;

namespace X.Abp.Account.Web.ExtensionGrants;

public class LinkLoginExtensionGrantProcessJsonResponse : IOpenIddictServerHandler<ApplyTokenResponseContext>
{
    public static OpenIddictServerHandlerDescriptor Descriptor => OpenIddictServerHandlerDescriptor.CreateBuilder<ApplyTokenResponseContext>().AddFilter<RequireHttpRequest>().UseSingletonHandler<LinkLoginExtensionGrantProcessJsonResponse>()
        .SetOrder(ProcessJsonResponse<ApplyTokenResponseContext>.Descriptor.Order - 1)
        .SetType(OpenIddictServerHandlerType.Custom)
        .Build();

    public ValueTask HandleAsync(ApplyTokenResponseContext context)
    {
        Check.NotNull(context, nameof(context));
        Check.NotNull(context.Transaction.Response!, nameof(context.Transaction.Response));
        if (context.Transaction.Properties.TryGetValue(typeof(AuthenticationProperties).FullName!, out var value))
        {
            var properties = value?.As<AuthenticationProperties>();
            if (properties != null && properties.Parameters.TryGetValue(LinkLoginExtensionGrant.TenantDomainParameterName, out var _))
            {
                context.Transaction.Response!.AddParameter(
                    LinkLoginExtensionGrant.TenantDomainParameterName,
                    new OpenIddictParameter(properties.GetParameter<string>(LinkLoginExtensionGrant.TenantDomainParameterName)));
            }
        }

        return default;
    }
}
