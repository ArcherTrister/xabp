// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

using X.Abp.Account.Localization;

namespace X.Abp.Account.Public.Web.Pages.Account.LinkUsers;

public class LinkUsersModalModel : AbpPageModel
{
    public LinkUsersModalModel()
    {
        LocalizationResourceType = typeof(AccountResource);
    }

    public virtual Task<IActionResult> OnGetAsync()
    {
        return Task.FromResult<IActionResult>(Page());
    }

    public virtual Task<IActionResult> OnPostAsync()
    {
        return Task.FromResult<IActionResult>(Page());
    }
}
