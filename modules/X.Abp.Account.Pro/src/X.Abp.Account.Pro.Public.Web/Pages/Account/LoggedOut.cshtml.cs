// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace X.Abp.Account.Public.Web.Pages.Account;

public class LoggedOutModel : AccountPageModel
{
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string ClientName { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string SignOutIframeUrl { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string PostLogoutRedirectUri { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string Culture { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string UICulture { get; set; }

    public virtual Task<IActionResult> OnGetAsync()
    {
        return Task.FromResult<IActionResult>(Page());
    }

    public virtual Task<IActionResult> OnPostAsync()
    {
        return Task.FromResult<IActionResult>(Page());
    }
}
