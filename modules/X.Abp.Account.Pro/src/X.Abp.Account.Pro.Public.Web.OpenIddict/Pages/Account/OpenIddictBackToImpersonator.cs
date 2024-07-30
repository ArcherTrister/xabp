// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Security.Principal;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;
using Volo.Abp.Security.Claims;

using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace X.Abp.Account.Public.Web.Pages.Account;

[ExposeServices(typeof(BackToImpersonatorModel))]
public class OpenIddictBackToImpersonatorModel : BackToImpersonatorModel
{
    protected AbpAccountOpenIddictOptions Options { get; }

    public OpenIddictBackToImpersonatorModel(
        ICurrentPrincipalAccessor currentPrincipalAccessor,
        SignInManager<IdentityUser> signInManager,
        IdentityUserManager userManager,
        IOptions<AbpAccountOpenIddictOptions> options)
        : base(currentPrincipalAccessor, signInManager, userManager)
    {
        Options = options.Value;
    }

    public override async Task<IActionResult> OnGetAsync()
    {
        if (!Request.Query.TryGetValue("access_token", out var _))
        {
            return await base.OnGetAsync();
        }

        var authenticateResult = await HttpContext.AuthenticateAsync(Options.ImpersonationAuthenticationScheme);
        if (authenticateResult.Succeeded)
        {
            using (CurrentPrincipalAccessor.Change(authenticateResult.Principal))
            {
                var guid = CurrentPrincipalAccessor.Principal.FindImpersonatorTenantId();
                var guid2 = CurrentPrincipalAccessor.Principal.FindImpersonatorUserId();
                if ((guid.HasValue || guid2.HasValue) && guid2.HasValue)
                {
                    using (CurrentTenant.Change(guid, null))
                    {
                        var user = await UserManager.GetByIdAsync(guid2.Value);
                        try
                        {
                            return await OpenIddictAuthorizeResponse.GenerateAuthorizeResponseAsync(HttpContext, user);
                        }
                        catch (Exception ex)
                        {
                            Logger.LogException(ex);
                            return Unauthorized();
                        }
                    }
                }

                return Unauthorized();
            }
        }

        return Unauthorized();
    }
}
