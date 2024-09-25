// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

using Volo.Abp.OpenIddict;
using Volo.Abp.OpenIddict.Controllers;

using X.Abp.Account.Web.ViewModels.Verify;

namespace X.Abp.Account.Web.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
[Route("connect/verify")]
[Authorize]
public class VerifyController : AbpOpenIdDictControllerBase
{
  [HttpGet]
  public virtual async Task<IActionResult> GetAsync()
  {
    var request = await GetOpenIddictServerRequestAsync(HttpContext);
    if (string.IsNullOrEmpty(request.UserCode))
    {
      return View("Verify", new VerifyViewModel());
    }

    var result = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    if (!result.Succeeded)
    {
      return View("Verify", new VerifyViewModel
      {
        Error = OpenIddictConstants.Errors.InvalidToken,
        ErrorDescription = "The specified user code is not valid. Please make sure you typed it correctly."
      });
    }

    var application = (await ApplicationManager.FindByClientIdAsync(result.Principal.GetClaim(OpenIddictConstants.Claims.ClientId))) ?? throw new InvalidOperationException("Details concerning the calling client application cannot be found.");
    VerifyViewModel verifyViewModel = new()
    {
      ApplicationName = await ApplicationManager.GetLocalizedDisplayNameAsync(application),
      Scope = string.Join(" ", result.Principal.GetScopes()),
      UserCode = request.UserCode
    };
    return View("Verify", verifyViewModel);
  }

  [HttpPost]
  public virtual async Task<IActionResult> PostAsync()
  {
    if (await HasFormValueAsync("deny"))
    {
      return Forbid(new AuthenticationProperties
      {
        RedirectUri = "/"
      },
      new string[1] { OpenIddictServerAspNetCoreDefaults.AuthenticationScheme });
    }

    var user = (await UserManager.GetUserAsync(User)) ?? throw new InvalidOperationException("The user details cannot be retrieved.");
    var result = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    if (!result.Succeeded)
    {
      return View("Verify", new VerifyViewModel
      {
        Error = OpenIddictConstants.Errors.InvalidToken,
        ErrorDescription = "The specified user code is not valid. Please make sure you typed it correctly."
      });
    }

    var principal = await SignInManager.CreateUserPrincipalAsync(user);
    principal.SetScopes(result.Principal.GetScopes());
    principal.SetResources(await ScopeManager.ListResourcesAsync(principal.GetScopes()).ToListAsync());

    await OpenIddictClaimsPrincipalManager.HandleAsync(await GetOpenIddictServerRequestAsync(HttpContext), principal);

    AuthenticationProperties properties = new()
    {
      RedirectUri = "/"
    };
    return SignIn(principal, properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
  }
}
