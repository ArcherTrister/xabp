// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using OpenIddict.Server.AspNetCore;

namespace X.Abp.OpenIddict.Controllers;

[Route("connect/logout")]
[ApiExplorerSettings(IgnoreApi = true)]
public class LogoutController : AbpOpenIdDictControllerBase
{
    [HttpGet]
    public virtual async Task<IActionResult> GetAsync()
    {
        // Ask ASP.NET Core Identity to delete the local and external cookies created
        // when the user agent is redirected from the external identity provider
        // after a successful authentication flow (e.g Google or Facebook).
        await SignInManager.SignOutAsync();

        // Returning a SignOutResult will ask OpenIddict to redirect the user agent
        // to the post_logout_redirect_uri specified by the client application or to
        // the RedirectUri specified in the authentication properties if none was set.
        return SignOut(authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
}
