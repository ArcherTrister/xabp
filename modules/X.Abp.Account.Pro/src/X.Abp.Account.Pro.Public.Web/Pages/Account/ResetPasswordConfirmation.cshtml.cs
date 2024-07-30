// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace X.Abp.Account.Public.Web.Pages.Account;

[AllowAnonymous]
public class ResetPasswordConfirmationModel : AccountPageModel
{
    [BindProperty(SupportsGet = true)]
    public string ReturnUrl { get; set; }

    [BindProperty(SupportsGet = true)]
    public string ReturnUrlHash { get; set; }

    public virtual async Task<IActionResult> OnGetAsync()
    {
        ReturnUrl = await GetRedirectUrlAsync(ReturnUrl, ReturnUrlHash);

        return Page();
    }
}
