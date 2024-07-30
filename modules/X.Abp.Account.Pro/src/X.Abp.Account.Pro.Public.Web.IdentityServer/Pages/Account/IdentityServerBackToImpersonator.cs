// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Security.Principal;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;
using Volo.Abp.Security.Claims;

using X.Abp.Account.Public.Web.Pages.Account;

using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace X.Abp.Account.Web.Pages.Account;

[ExposeServices(typeof(BackToImpersonatorModel))]
public class IdentityServerBackToImpersonatorModel : BackToImpersonatorModel
{
    protected AbpAccountIdentityServerOptions Options { get; }

    public IdentityServerBackToImpersonatorModel(
        ICurrentPrincipalAccessor currentPrincipalAccessor,
        SignInManager<IdentityUser> signInManager,
        IdentityUserManager userManager,
        IOptions<AbpAccountIdentityServerOptions> options)
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
                var impersonatorTenantId = CurrentPrincipalAccessor.Principal.FindImpersonatorTenantId();
                var impersonatorUserId = CurrentPrincipalAccessor.Principal.FindImpersonatorUserId();

                if ((impersonatorTenantId == null && impersonatorUserId == null) || impersonatorUserId == null)
                {
                    return Unauthorized();
                }

                using (CurrentTenant.Change(impersonatorTenantId))
                {
                    var user = await UserManager.GetByIdAsync(impersonatorUserId.Value);
                    try
                    {
                        await IdentityServerAuthorizeResponse.GenerateAuthorizeResponseAsync(HttpContext, user);
                    }
                    catch (Exception e)
                    {
                        Logger.LogException(e);
                        return Unauthorized();
                    }

                    return new EmptyResult();
                }
            }
        }

        return Unauthorized();
    }
}
