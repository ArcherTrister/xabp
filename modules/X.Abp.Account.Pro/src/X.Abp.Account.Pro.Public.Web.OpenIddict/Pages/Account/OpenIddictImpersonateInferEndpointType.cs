// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.AspNetCore;

using OpenIddict.Server;
using OpenIddict.Server.AspNetCore;

using Volo.Abp;

namespace X.Abp.Account.Public.Web.Pages.Account;

public class OpenIddictImpersonateInferEndpointType : IOpenIddictServerHandler<OpenIddictServerEvents.ProcessRequestContext>
{
    public static OpenIddictServerHandlerDescriptor Descriptor => OpenIddictServerHandlerDescriptor.CreateBuilder<OpenIddictServerEvents.ProcessRequestContext>()
                .AddFilter<OpenIddictServerAspNetCoreHandlerFilters.RequireHttpRequest>()
                .UseSingletonHandler<OpenIddictImpersonateInferEndpointType>()
                .SetOrder(OpenIddictServerHandlers.InferEndpointType.Descriptor.Order + 1)
                .SetType(OpenIddictServerHandlerType.Custom)
                .Build();

    private static readonly List<string> ImpersonatePaths = new()
    {
       "Account/ImpersonateTenant",
       "Account/ImpersonateUser",
       "Account/BackToImpersonator",
       "Account/DelegatedImpersonate"
    };

    public virtual ValueTask HandleAsync(OpenIddictServerEvents.ProcessRequestContext context)
    {
        Check.NotNull(context, nameof(context));
        var httpRequest = context.Transaction.GetHttpRequest();
        Check.NotNull(httpRequest, nameof(httpRequest));

        if (context.EndpointType == OpenIddictServerEndpointType.Authorization
        && httpRequest.HasFormContentType
        && context.RequestUri != null
        && ImpersonatePaths.Any(impersonateUri => context.RequestUri.PathAndQuery.Contains(impersonateUri, StringComparison.OrdinalIgnoreCase)))
        {
            context.SkipRequest();
        }

        return new ValueTask();
    }
}
