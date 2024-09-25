// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Security.Principal;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using Volo.Abp.Identity;
using Volo.Abp.Security.Claims;

namespace X.Abp.Account.Public.Web.Pages.Account;

[Authorize]
[IgnoreAntiforgeryToken]
public class BackToImpersonatorModel : AccountPageModel
{
    protected ICurrentPrincipalAccessor CurrentPrincipalAccessor { get; }

    public BackToImpersonatorModel(ICurrentPrincipalAccessor currentPrincipalAccessor)
    {
        CurrentPrincipalAccessor = currentPrincipalAccessor;
    }

    public virtual Task<IActionResult> OnGetAsync()
    {
        return Task.FromResult<IActionResult>(NotFound());
    }

    public virtual async Task<IActionResult> OnPostAsync()
    {
        var impersonatorTenantId = CurrentPrincipalAccessor.Principal.FindImpersonatorTenantId();
        var impersonatorUserId = CurrentPrincipalAccessor.Principal.FindImpersonatorUserId();

        if ((impersonatorTenantId == null && impersonatorUserId == null) || impersonatorUserId == null)
        {
            return await RedirectSafelyAsync("~/");
        }

        using (CurrentTenant.Change(impersonatorTenantId))
        {
            var user = await UserManager.GetByIdAsync(impersonatorUserId.Value);
            var isPersistent = (await HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme))?.Properties?.IsPersistent ?? false;
            await SignInManager.SignOutAsync();

            await SignInManager.SignInAsync(user, new AuthenticationProperties
            {
                IsPersistent = isPersistent
            });
            await IdentityDynamicClaimsPrincipalContributorCache.ClearAsync(user.Id, user.TenantId);
            return Redirect("~/");
        }
    }
}
