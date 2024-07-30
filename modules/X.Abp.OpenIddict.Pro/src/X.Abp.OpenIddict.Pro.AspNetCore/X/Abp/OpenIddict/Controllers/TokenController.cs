// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using OpenIddict.Abstractions;

using Volo.Abp;

using X.Abp.OpenIddict.ExtensionGrantTypes;

namespace X.Abp.OpenIddict.Controllers;

[Route("connect/token")]
[IgnoreAntiforgeryToken]
[ApiExplorerSettings(IgnoreApi = true)]
public partial class TokenController : AbpOpenIdDictControllerBase
{
    [HttpGet]
    [HttpPost]
    [Produces("application/json")]
    public virtual async Task<IActionResult> HandleAsync()
    {
        var request = await GetOpenIddictServerRequestAsync(HttpContext);

        if (request.IsPasswordGrantType())
        {
            return await HandlePasswordAsync(request);
        }

        if (request.IsAuthorizationCodeGrantType())
        {
            return await HandleAuthorizationCodeAsync(request);
        }

        if (request.IsRefreshTokenGrantType())
        {
            return await HandleRefreshTokenAsync(request);
        }

        if (request.IsDeviceCodeGrantType())
        {
            return await HandleDeviceCodeAsync(request);
        }

        if (request.IsClientCredentialsGrantType())
        {
            return await HandleClientCredentialsAsync(request);
        }

        var extensionGrantsOptions = HttpContext.RequestServices.GetRequiredService<IOptions<AbpOpenIddictExtensionGrantsOptions>>();
        var extensionTokenGrant = extensionGrantsOptions.Value.Find<ITokenExtensionGrant>(request.GrantType);
        return extensionTokenGrant != null
            ? await extensionTokenGrant.HandleAsync(new ExtensionGrantContext(HttpContext, request))
            : throw new AbpException(string.Format(L["TheSpecifiedGrantTypeIsNotImplemented"], request.GrantType));
    }
}
