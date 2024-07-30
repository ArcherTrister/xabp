// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using Volo.Abp.Identity;

using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace X.Abp.Account.Public.Web.Pages.Account;

public class LogoutModel : AccountPageModel
{
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string ReturnUrl { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string ReturnUrlHash { get; set; }

    protected SignInManager<IdentityUser> SignInManager { get; }

    protected IdentitySecurityLogManager IdentitySecurityLogManager { get; }

    public LogoutModel(SignInManager<IdentityUser> signInManager, IdentitySecurityLogManager identitySecurityLogManager)
    {
        SignInManager = signInManager;
        IdentitySecurityLogManager = identitySecurityLogManager;
    }

    public virtual async Task<IActionResult> OnGetAsync()
    {
        if (CurrentUser.IsAuthenticated)
        {
            await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
            {
                Identity = IdentitySecurityLogIdentityConsts.Identity,
                Action = IdentitySecurityLogActionConsts.Logout
            });
        }

        await SignInManager.SignOutAsync();
        await HttpContext.SignOutAsync(ConfirmUserModel.ConfirmUserScheme);

        return ReturnUrl != null ? await RedirectSafelyAsync(ReturnUrl, ReturnUrlHash) : RedirectToPage("~/Account/Login");
    }

    public virtual Task<IActionResult> OnPostAsync()
    {
        return Task.FromResult<IActionResult>(Page());
    }
}
