// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Immutable;
using System.Security.Claims;
using System.Security.Principal;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using OpenIddict.Abstractions;

using Volo.Abp.AspNetCore.WebClientInfo;
using Volo.Abp.OpenIddict;
using Volo.Abp.OpenIddict.ExtensionGrantTypes;

using X.Abp.Identity;

namespace X.Abp.Account.Web.ExtensionGrants;

public abstract class TokenExtensionGrantBase : ITokenExtensionGrant
{
    public abstract string Name { get; }

    protected virtual Task<Guid?> GetRawValueOrNullAsync(OpenIddictRequest request, string key)
    {
        var text = request.GetParameter(key).ToString();
        return text.IsNullOrWhiteSpace()
            ? Task.FromResult<Guid?>(null)
            : !Guid.TryParse(text, out var result) ? Task.FromResult<Guid?>(null) : Task.FromResult((Guid?)result);
    }

    protected virtual async Task<IEnumerable<string>> GetResourcesAsync(ExtensionGrantContext context, ImmutableArray<string> scopes)
    {
        List<string> resources = new();
        if (!scopes.Any())
        {
            return resources;
        }

        var asyncEnumerator = context.HttpContext.RequestServices.GetRequiredService<IOpenIddictScopeManager>().ListResourcesAsync(scopes).GetAsyncEnumerator();
        try
        {
            while (await asyncEnumerator.MoveNextAsync())
            {
                var current = asyncEnumerator.Current;
                resources.Add(current);
            }
        }
        finally
        {
            if (asyncEnumerator != null)
            {
                await asyncEnumerator.DisposeAsync();
            }
        }

        return resources;
    }

    protected virtual async Task SetClaimsDestinationsAsync(ExtensionGrantContext context, ClaimsPrincipal principal)
    {
        await context.HttpContext.RequestServices.GetRequiredService<AbpOpenIddictClaimsPrincipalManager>().HandleAsync(context.Request, principal);
    }

    protected virtual async Task CreateSessionAsync(ExtensionGrantContext context, ClaimsPrincipal principal)
    {
        string sessionId = principal.FindSessionId();
        if (sessionId.IsNullOrWhiteSpace())
        {
            return;
        }

        IdentitySessionManager identitySessionManager = context.HttpContext.RequestServices.GetRequiredService<IdentitySessionManager>();
        IOptions<AbpAccountOpenIddictOptions> accountOpenIddictOptions = context.HttpContext.RequestServices.GetRequiredService<IOptions<AbpAccountOpenIddictOptions>>();
        IWebClientInfoProvider webClientInfoProvider = context.HttpContext.RequestServices.GetRequiredService<IWebClientInfoProvider>();
        if (context.Request.ClientId.IsNullOrWhiteSpace() || !accountOpenIddictOptions.Value.ClientIdToDeviceMap.TryGetValue(context.Request.ClientId, out var device))
        {
            device = "OAuth";
        }

        await identitySessionManager.CreateAsync(sessionId, device, webClientInfoProvider.DeviceInfo, principal.FindUserId().Value, principal.FindTenantId(), context.Request.ClientId, webClientInfoProvider.ClientIpAddress);
    }

    public abstract Task<IActionResult> HandleAsync(ExtensionGrantContext context);
}
